# Payment Gateway

# General comments

The solution has the following things implemented:

- All four endpoints written (as POST endpoints)
- An In-Memory DB with:
    - A simple adapter with needed functions to interact with DB
    - DTOs (and their mapping) for the Authorise and the Capture/Refund requests
- Request body validation using FluentValidation
- Quick logging using NLog. Logs are set to be stored in the bin folder.
- Some unit tests using XUnit, AutoFixtures, FluentAssertions

## Assumptions

### Data

Data assumptions are defined by the validators and the models used throughout. For example, CC# is a string, Currency is handled as an enum, amounts as doubles, CVV/exp. month/exp. date as integers.

### Logic

The amounts and the transactions are handled internally in the DB. There are no external connections, mocked or otherwise.

Each flow is a Transaction. It has a Status flag with the following options:

- Active (set when created, remains Active throughout each Capture)
- Refund (set once a Refund is processed)
- Finalised - set when:
    - Refunded amount equals captured amount; technically no further TransactionItems can occur given current spec. Current tolerance is 0.0001
    - After a sufficient amount of time passes between last Capture and first Refund (not implemented, but nice to have)
- Void (when /void is called)

The Authorisation call creates the transaction and each Capture/Refund is a TransactionItem operation.

Luhn check is implemented by FluentValidator:

```csharp
RuleFor(req => req.CardNumber).CreditCard();
```

## Areas to improve

- More unit tests. Better unit tests. There's always room to improve.
- Add timestamps in DB. Additionally, create a second table, used as a Transaction history, logging each action performed on a transaction: capture, refund, etc.
- Do more concurrency tests. I hadn't had the time to stress test the app and I believe there could be concurrency issues when trying to do multiple queries in quick succession on the same transaction. Using ETags should help with that. Perhaps this can be implemented alongside point above, i.e checking for the last Item in the flow of a transaction.
- Failures requested in the spec are too generic and I feel I have implemented them poorly implemented in the adapter. I'm assuming they're just there to fake a generic failure, hence the generic error thrown.
