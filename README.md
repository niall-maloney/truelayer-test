# Truelayer Technical Test

https://gist.githubusercontent.com/Neku/cb63ee836960cd9566d3291e4114d4a9/raw/f10d89187d46d311fb14039e9edd1b5d0fcdedf3/test

## Pre-requisities

- dotnet core 2.0 sdk
- docker
- TrueLayer

## Environment variables

Environment variables needed to run the application are taken up from true-layer.env file.

## Run application

1. Copy `sample.env` file in application root directory to a new file named `.env`. Update environment variables in `.env` with your client id and client secret.
2. Run the following commands
```
$ docker-compose build
$ docker-compose up
```
3. Navigate to https://localhost:5000/api/v1/transactions in your browser.

## Run unit tests
```
$ sh run-tests.sh
```

## Using the API

`GET /api/v1/transactions`
- Returns all of the users transactions across accounts.

`GET /api/v1/transactions/summary`
- Returns a summary of statistics for all transaction categories.