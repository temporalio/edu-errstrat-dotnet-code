# Exercise #3: Rollback with the Saga Pattern

During this exercise, you will:

- Orchestrate Activities using a Saga pattern to implement compensating transactions
- Handle failures with rollback logic

Make your changes to the code in the `practice` subdirectory (look for `TODO` comments that will guide you to where you should make changes to the code). If you need a hint or want to verify your changes, look at the complete version in the `solution` subdirectory.

## Part A: Review your rollback Activities

This Exercise uses the same structure as in the previous Exercises — meaning  that it will fail at the very end on the `ValidateCreditCard` Activity if you provide it with a bad credit card number.

Three new Activities have been created to demonstrate rollback actions.

* `UpdateInventory` reduces the stock from the pizza inventory once the pizza order comes through.
* `RevertInventory` has also been added as a compensating action for `UpdateInventory`. It add the ingredients back into the pizza inventory.
* `RefundCustomer` has been added as a compensating action for `SendBill`.

1. Review these new Activities in `Activities.cs` in the `Workflow` directory. None of them make actual inventory or billing changes, because the intent of this Activity is to show Temporal features, but you should be able to see where you could add functionality here.
2. Close the files.

## Part B: Add your new rollback Activities to your Workflow

Now you will implement a compensating action using Activities in your Temporal Workflow. 

1. Open `PizzaWorkflow.cs` from your `Workflow` directory.
2. Note that a List, `compensations`, has been added at the top to keep track of each Activity's compensating action.
3. Note that after the bill is created in the `PizzaWorkflow` file, the `UpdateInventory` Activity is executed, before the `SendBill` Activity is called. The compensating action was added to the compensations list in the list above. Study this and use it for the next step.
4. Locate the invocation for the `SendBill` Activity. Add the appropriate compensating Activity to the compensations list, containing the compensating input. Use the previous step as a reference.

## Part C: Create Your `Compensate` Function

In this part of the exercise, you will create a function which will loop through the each one of the items in your `compensations` list in a synchronous order. In the case of an error, we will invoke this function to roll back on any Activities we want to undo.

1. In the `PizzaWorkflow.cs` file, locate the `Compensate` Task after the `PizzaWorkflow`.
2. Call the `Reverse` method before you loop through the `compensations` list. This ensures that compensating actions are called in reverse order, aligning with the correct sequence to roll back local transactions that have already completed.

## Part D: Add your `Compensate` Task to your Workflow

In this part of the exercise, you will call the `Compensate` function that you defined in Part C.

1. In the `PizzaWorkflow.cs` file, notice you have a `try/catch` block. You call your Activities in the `try` block. In the `catch` block, if an error occurs, we want to roll back all of the Activities that have so far executed by calling the compensating actions.
2. In the `catch` block of the `PizzaWorkflow`, call `await CompensateAsync()`. Now if `ValidateCreditCard` fails, first we roll back on `SendBill` by calling `RefundCustomer`. Next, we will roll back on `UpdateInventory` by calling `RevertInventory`.
3. Save the file.

## Part E: Test the Rollback of Your Activities

To run the Workflow:

1. In one terminal, start the Worker by running `dotnet run --project Worker`.
2. In another terminal, start the Workflow by running `dotnet run --project Client`.
3. You should see the Workflow Execution failed. There is now a `WorkflowExecutionFailed` Event in the Web UI.
4. Over in the Web UI (or the terminal window where your Worker ran), you can see that after the `ValidateCreditCard` Activity failed, we then called the Activities: `RefundCustomer` and `RevertInventory`.

### This is the end of the exercise.