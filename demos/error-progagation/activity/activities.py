from typing import NoReturn

from temporalio import activity
from temporalio.exceptions import ApplicationError


@activity.defn
async def compose_greeting(name: str) -> NoReturn:
    # Always raise exception
    raise ApplicationError(
        f"Failure from Python Activity: Hello {name}!", non_retryable=True
    )
