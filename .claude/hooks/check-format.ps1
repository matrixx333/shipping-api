#!/usr/bin/env pwsh
#Requires -Version 7
<#
    Stop hook — runs `dotnet format` over the C# files touched in this session.

    This is the second, slower enforcement layer. `dotnet format` cannot check
    line length (Roslyn has no reflow-to-column-limit pass), but it does catch
    the spacing/indentation class of problem that check-line-length.ps1 misses.
    It costs ~3.3s, which is why it runs once at end of turn rather than after
    every edit.

    Scoped deliberately to session-changed files: the repo carries pre-existing
    whitespace debt, and scoping avoids blocking on code this session never
    touched.

    Exit codes:
      0  pass — nothing to say
      2  block — stderr is fed back to the agent as actionable feedback
#>

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

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

if (-not [string]::IsNullOrWhiteSpace($raw))
{
    try
    {
        $payload = $raw | ConvertFrom-Json

        # Already blocked a stop once; blocking again risks an infinite loop.
        if ($payload.PSObject.Properties.Name -contains 'stop_hook_active' -and
            $payload.stop_hook_active)
        {
            exit 0
        }
    }
    catch
    {
        # Unparseable payload — never block on our own parsing failure.
    }
}

# --- preconditions ----------------------------------------------------------

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) { exit 0 }

$repoRoot = & git rev-parse --show-toplevel 2>$null
if ($LASTEXITCODE -ne 0 -or -not $repoRoot) { exit 0 }

$repoRoot = ($repoRoot | Select-Object -First 1).Trim() -replace '\\', '/'

Push-Location -LiteralPath $repoRoot

try
{
    # --- which C# files changed this session? -------------------------------

    $changed = @(& git diff --name-only HEAD -- '*.cs' 2>$null)
    $added   = @(& git ls-files --others --exclude-standard -- '*.cs' 2>$null)

    $targets = @($changed + $added |
        Where-Object { $_ } |
        Where-Object { Test-InScope $_ } |
        Sort-Object -Unique |
        Where-Object { Test-Path -LiteralPath $_ })

    # Nothing to check — skip dotnet format entirely and stay fast.
    if ($targets.Count -eq 0) { exit 0 }

    # --- verify -------------------------------------------------------------

    $solution = Join-Path $repoRoot 'shipping-api.sln'
    $arguments = @('format', 'whitespace')

    if (Test-Path -LiteralPath $solution) { $arguments += $solution }

    $arguments += '--verify-no-changes'
    $arguments += '--include'
    $arguments += $targets

    $output = & dotnet @arguments 2>&1
    if ($LASTEXITCODE -eq 0) { exit 0 }

    $issues = @($output | Where-Object { $_ -match 'error WHITESPACE' })

    # Non-zero for some other reason (build error, bad args) — not a style
    # problem, so do not block the turn on it.
    if ($issues.Count -eq 0) { exit 0 }

    $lines = @(
        "[dotnet format] $($issues.Count) whitespace issue(s) in files changed this session:"
        ''
        ($issues | ForEach-Object { "  $_" })
        ''
        'Fix with:'
        "  dotnet format whitespace shipping-api.sln --include $($targets -join ' ')"
        ''
        'This is safe to auto-apply — dotnet format parses the code with Roslyn.'
    )

    [Console]::Error.WriteLine(($lines | Out-String).TrimEnd())
    exit 2
}
finally
{
    Pop-Location
}
