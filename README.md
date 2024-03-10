# Overview

This ASP.NET Core Web API project was created to demonstrate how I solved a customer's real world problem. The customer had a database that contained shipping addresses for customers. It also contained the account credentials for the shipping providers that they used (Fed Ex, UPS, etc.)

They wanted a way to be able to validate addresses, get rates on shipping, and print shipping labels from these various shipping providers using their internal web application. This code only demonstrates the ability to validate a shipping address, but, it was originally developed to solve the all of the customer's requests. 

Each shipping provider uses their own schema definition when it comes to submitting requests to their API. Therefore, it was necessary to develop a solution that allowed each of the API request objects to be built in a unique way.

In the end, the internal web application would create a POST request to the customer's shipping API (this project). This in turn would submit a request to the shipping provider, then the response was formatted and delivered back to the internal web application for the customer to be able to view the result. 

# Project Structure 

## Extenions

This folder is a place to aggregate IServiceCollection extension methods.

## HttpClients

This folder contains the http clients that have been configured to call each shipping providers unique API endpoint URI. 

The API documentation that was used can be found at the following locations: 

UPS Address Validation: https://developer.ups.com/api/reference?loc=en_US#operation/AddressValidation

Fed Ex Address Validation: https://developer.fedex.com/api/en-us/catalog/address-validation/v1/docs.html#operation/Validate%20Address

## Models

These represent the POCO objects that are returned from the customer's database that stores both the shipping addresses, and shipping company account details.

## Services

These are the service objects that have a dependency on the database. They are able to retrieve database records provided a given key to search by.

## Program.cs

This is the main entry point into the API. There is a single POST endpoint ('validate-address') for validating addresses. The request object that is sent as part of the POST contains the shippingCompanyId for the specific shipping provider along with an addressId that is used to obtain the address that is stored in the database. The address record is retrieved using the addressId, then it is built as part of the API request that is sent to the shipping provider. 

# Disclaimer 

This code currently does not actually connect to a shipping providers API. I do not have accounts with Fed Ex or UPS, therefore, no calls can be authenticated. 
