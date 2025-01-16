package greetingworkflow;

import io.temporal.activity.ActivityOptions;
import io.temporal.workflow.Workflow;
import io.temporal.failure.ActivityFailure;
import io.temporal.failure.ApplicationFailure;
import io.temporal.workflow.ActivityStub;

import java.time.Duration;
import java.util.List;

import org.slf4j.Logger;

public class GreetingWorkflowImpl implements GreetingWorkflow {

  public static final Logger logger = Workflow.getLogger(GreetingWorkflowImpl.class);

  ActivityOptions options =
      ActivityOptions.newBuilder().setStartToCloseTimeout(Duration.ofSeconds(5)).build();

  @Override
  public String getGreeting(String name) {

    // Define activity options and get ActivityStub
    ActivityStub activity =
    Workflow.newUntypedActivityStub(
        ActivityOptions.newBuilder().setStartToCloseTimeout(Duration.ofSeconds(10)).build());

    String greetingResult;
    try{
      greetingResult = activity.execute("compose_greeting", String.class, name);
    } catch (ActivityFailure e){
      throw ApplicationFailure.newFailureWithCause("Failure from Java Workflow", "ErrorPropagationDemo", e);
    }
    

    return greetingResult;
  }
}
