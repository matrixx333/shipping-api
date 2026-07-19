#!/usr/bin/env pwsh
#Requires -Version 7
<#
    PostToolUse hook — enforces the 100-character line limit documented in
    .claude/rules/code-style.md

    Reads the hook payload as JSON on stdin and inspects only the file that was
    just edited, so the check costs ~200ms rather than scanning the repo.

    Exit codes:
      0  pass — nothing to say
      2  block — stderr is fed back to the agent as actionable feedback

    Lines that were already over the limit in HEAD never block on their own. The
    first time they are encountered the hook asks the agent to get a scope
    decision from the user, then honours whatever is recorded in
    .claude/.state/line-length-scope from then on.
#>

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$MaxLength = 100

function Get-VisualLength([string]$Line)
{
    # Tabs count as 4 columns, per the rule.
    ($Line -replace "`t", '    ').Length
}

function Get-LongLines([string[]]$Lines)
{
    $found = [System.Collections.Generic.List[object]]::new()

    for ($i = 0; $i -lt $Lines.Count; $i++)
    {
        $length = Get-VisualLength $Lines[$i]

        if ($length -gt $MaxLength)
        {
            $found.Add(
                [pscustomobject]@{
                    Number = $i + 1
                    Length = $length
                    Text   = $Lines[$i].Trim()
                    Key    = $Lines[$i].Trim()
                })
        }
    }

    # Callers must wrap this in @() — an empty result unrolls to $null and a
    # single result unrolls to a scalar otherwise.
    return $found.ToArray()
}

function Test-InScope([string]$PathLike)
{
    $normalized = $PathLike -replace '\\', '/'

    if ($normalized -notmatch '\.cs$')    { return $false }
    if ($normalized -match '\.g\.cs$')    { return $false }
    if ($normalized -match '/(obj|bin)/') { return $false }

    return $true
}

# --- read the hook payload --------------------------------------------------

$raw = [Console]::In.ReadToEnd()
if ([string]::IsNullOrWhiteSpace($raw)) { exit 0 }

try
{
    $payload = $raw | ConvertFrom-Json
}
catch
{
    # Not a payload we understand; never block on our own parsing failure.
    exit 0
}

$filePath = $null

if ($payload.PSObject.Properties.Name -contains 'tool_input')
{
    $toolInput = $payload.tool_input

    if ($toolInput -and $toolInput.PSObject.Properties.Name -contains 'file_path')
    {
        $filePath = $toolInput.file_path
    }
}

if (-not $filePath)                             { exit 0 }
if (-not (Test-InScope $filePath))              { exit 0 }
if (-not (Test-Path -LiteralPath $filePath))    { exit 0 }

# --- locate the repo and the file within it ---------------------------------

$repoRoot = & git rev-parse --show-toplevel 2>$null
if ($LASTEXITCODE -ne 0 -or -not $repoRoot) { exit 0 }

$repoRoot = ($repoRoot | Select-Object -First 1).Trim() -replace '\\', '/'
$absolute = (Resolve-Path -LiteralPath $filePath).Path -replace '\\', '/'
$relative = $absolute

if ($absolute.StartsWith($repoRoot, [StringComparison]::OrdinalIgnoreCase))
{
    $relative = $absolute.Substring($repoRoot.Length).TrimStart('/')
}

# --- find violations in the working copy ------------------------------------

$violations = @(Get-LongLines ([System.IO.File]::ReadAllLines($absolute)))
if ($violations.Count -eq 0) { exit 0 }

# --- classify: which of these were already in HEAD? -------------------------

$headLines = & git show "HEAD:$relative" 2>$null
$isTracked = ($LASTEXITCODE -eq 0)

$legacyKeys = [System.Collections.Generic.HashSet[string]]::new(
    [StringComparer]::Ordinal)

if ($isTracked -and $headLines)
{
    foreach ($line in @(Get-LongLines ([string[]]$headLines)))
    {
        [void]$legacyKeys.Add($line.Key)
    }
}

$new    = @($violations | Where-Object { -not $legacyKeys.Contains($_.Key) })
$legacy = @($violations | Where-Object { $legacyKeys.Contains($_.Key) })

function Format-Violations($Items)
{
    $Items | ForEach-Object {
        "  {0}:{1}  ({2} chars)`n      {3}" -f $relative, $_.Number, $_.Length, $_.Text
    }
}

# --- newly written violations always block ----------------------------------

if ($new.Count -gt 0)
{
    $lines = @(
        "[code-style] $($new.Count) line(s) exceed the $MaxLength-character limit:"
        ''
        (Format-Violations $new)
        ''
        'Rewrap these per .claude/rules/code-style.md before continuing.'
    )

    [Console]::Error.WriteLine(($lines | Out-String).TrimEnd())
    exit 2
}

# --- only pre-existing violations remain ------------------------------------

$statePath = Join-Path $repoRoot '.claude/.state/line-length-scope'
$scope     = $null

if (Test-Path -LiteralPath $statePath)
{
    $scope = (Get-Content -LiteralPath $statePath -Raw).Trim()
}

if ($scope -eq 'new-only') { exit 0 }

if ($scope -eq 'touched-files' -or $scope -eq 'whole-codebase')
{
    $lines = @(
        "[code-style] $($legacy.Count) pre-existing line(s) over $MaxLength characters " +
            "in a file you just edited (scope: $scope):"
        ''
        (Format-Violations $legacy)
        ''
        'Rewrap these per .claude/rules/code-style.md.'
    )

    [Console]::Error.WriteLine(($lines | Out-String).TrimEnd())
    exit 2
}

# --- no decision on record: the legacy-codebase case ------------------------

$totalFiles = 0
$totalLines = 0

$candidates = Get-ChildItem -LiteralPath $repoRoot -Recurse -Filter *.cs -File |
    Where-Object { Test-InScope $_.FullName }

foreach ($candidate in $candidates)
{
    $count = @(Get-LongLines ([System.IO.File]::ReadAllLines($candidate.FullName))).Count

    if ($count -gt 0)
    {
        $totalFiles++
        $totalLines += $count
    }
}

$prompt = @"
[code-style] Scope decision needed before continuing.

$relative has $($legacy.Count) line(s) over $MaxLength characters, but none of them were
written in this session. Repo-wide: $totalLines violation(s) across $totalFiles file(s).

No scope decision is on record. Use AskUserQuestion to ask:

  "This repo has $totalLines pre-existing lines over $MaxLength characters across
   $totalFiles files, from before the style rule existed. How should the rule be applied?"

    - "New code only"         enforce on lines written from now on; leave legacy alone
    - "Files as I touch them" clean a file's violations whenever it is edited (boy-scout)
    - "Whole codebase now"    one-time sweep of all $totalFiles files, as its own commit

Then record the answer by writing exactly one of ``new-only``, ``touched-files``, or
``whole-codebase`` to .claude/.state/line-length-scope — this question is asked once.
"@

[Console]::Error.WriteLine($prompt.TrimEnd())
exit 2
