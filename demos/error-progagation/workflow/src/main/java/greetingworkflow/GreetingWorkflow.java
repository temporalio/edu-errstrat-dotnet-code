package greetingworkflow;

import io.temporal.workflow.WorkflowInterface;
import io.temporal.workflow.WorkflowMethod;

@WorkflowInterface
public interface GreetingWorkflow {

  /**
   * Define the parent workflow method. This method is executed when the workflow is started. The
   * workflow completes when the workflow method finishes execution.
   */
  @WorkflowMethod
  String getGreeting(String name);
}

