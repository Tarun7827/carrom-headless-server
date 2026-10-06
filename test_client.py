import asyncio
import json
import os
import websockets

REQUEST={"requestId":"local-test-001","striker":{"position":{"x":0,"y":-3.5},"velocity":{"x":2.5,"y":12}},"tokens":[],"maxSimulationSeconds":8}

async def main():
    port=os.getenv("CARROM_PORT","8080")
    async with websockets.connect(f"ws://127.0.0.1:{port}",max_size=4*1024*1024) as ws:
        await ws.send(json.dumps(REQUEST,separators=(",",":")))
        print(await ws.recv())

if __name__=="__main__":
    asyncio.run(main)
