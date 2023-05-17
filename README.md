# basket-api-dotnet

This is a shell of an api for a basket.

It provides a single endpoint for adding to a basket, which should return the recalculated basket.

It keeps track of the basket in memory, so it will be lost when the server is restarted. Just to keep things simple.

## Todo

1. Implement the basket api such that it can return the correct BasketState.

2. Add a remove endpoint that allow the user to remove items from the basket.