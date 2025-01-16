## Exercise #1: Handling Errors

During this exercise, you will:

- Throw and handle exceptions in Temporal Workflows and Activities
- Use non-retryable errors to fail an Activity
- Locate the details of a failure in Temporal Workflows and Activities in the Event History

Make your changes to the code in the `practice` subdirectory (look for `TODO`
comments that will guide you to where you should make changes to the code). If
you need a hint or want to verify your changes, look at the complete version in
the `solution` subdirectory.

## Part A: Throw a non-retryable `ApplicationFailureException` to fail an Activity

In this part of the exercise, you will throw a non-retryable Application Failure
that will fail your Activities.

Application Failures are used to communicate application-specific failures in
Workflows and Activities. Throwing an `ApplicationFailureException` from an Activity will ordinarily cause the Activity to fail and be retried. If you do not want a particular Activity to be retried upon failure, you can set it as non-retryable.

1. Start by reviewing the `Activities.cs` file in the `Workflow` directory, familiarizing yourself with the Activity functions.
2. Add `Temporalio.Exceptions` in your imports in `Activities.cs` at the top of your file.
3. In the `SendBill` Activity, a non-retryable `ApplicationFailureException` should be thrown if the charge is negative. Using a non-retryable failure ensures that the Activity fails immediately when the amount is calculated to be negative. The `ApplicationFailureException` includes a message and a details list, which can store information related to the failure. To serialize and transmit custom error data over the network, set the custom data in the `details` field. Include a `nonRetryable` key and set it to `true`.
4. Go to the `ValidateCreditCard` Activity. In the if statement if the credit card number does not have 16 digits, throw an `ApplicationFailureException`. In the details field, add your credit card number. Set the nonRetryable key to true. You can follow the pattern you saw in step 3.
5. Save your file.

## Part B: Throw the Activity Failure in Your Workflow

In this part of the exercise, you will catch the `ApplicationFailureException` that was thrown from the `ValidateCreditCard` Activity and handle it.

1. Edit the`PizzaWorkflow.cs` file from your `Workflow` directory.
2. Look at the call to the `ValidateCreditCard` Activity. 
   i. If a non-retryable Application Failure is thrown, the Workflow Execution will fail. However, it is possible to catch this failure and either handle it, or continue to propagate it up. 
   ii. We wrapped the call to the `ValidateCreditCard` Activity in a try/catch block. Since the `ApplicationFailureException` in the Activity is designated as non-retryable, by the time it reaches the Workflow it is converted to an ActvityFailure. 
   iii. Within the catch block, add a logging statement stating that the Activity has failed. 
3. After the logging statement, throw another `ApplicationFailureException`, passing 'Invalid credit card number error' in the message field and the credit card number in the details field. This will cause the Workflow to fail, as you were unable to bill the customer.
4. Save your file.

## Part C: Run the Workflow

In this part of the exercise, you will run your Workflow and see both your
Workflow and Activity succeed and fail.

In the `Program.cs` file in your Client directory, a `CreditCardNumber` parameter has been provided as part of the input to this Workflow. Right now, this Exercise code only validates the length of the `CreditCardNumber` -- meaning that a 16 digit string will pass, and a shorter string will fail.

**First, run the Workflow successfully:**

1. In one terminal, start the Worker by running `dotnet run --project Worker`.
2. In another terminal, start the Workflow by running `dotnet run --project Client`.
3. In the Web UI, verify that the Workflow ran successfully to completion.

**Next, you'll modify the Client data to cause the Workflow to fail:**

1. Open `Program.cs` file in your Client directory and modify the `CreditCardNumber` value in the `CreatePizzaOrder` function to be `1234`. Save this file.
2. In one terminal, restart the Worker by using `Ctrl-C` then re-running `dotnet run --project Worker`.
3. In another terminal, start the Workflow by running `dotnet run --project Client`.
4. You should see in the Web UI a `WorkflowExecutionFailed` Event with the message: "Invalid credit card number error".

### This is the end of the exercise.