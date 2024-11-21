import asyncio

from activities import compose_greeting
from temporalio.client import Client
from temporalio.worker import Worker


async def main():
    client = await Client.connect("localhost:7233", namespace="default")

    worker = Worker(
        client,
        task_queue="errors-demo",
        activities=[compose_greeting],
    )
    print("Starting the worker....")
    await worker.run()


if __name__ == "__main__":
    asyncio.run(main())
