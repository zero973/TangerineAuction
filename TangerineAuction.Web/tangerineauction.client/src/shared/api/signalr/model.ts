import * as signalR from "@microsoft/signalr";
import { createEffect, createEvent, sample } from "effector";
import { initAuthFx } from "@shared/api/auth/model";
import { HubMessage, parseHubMessage } from "@shared/api/contracts/hub";

let connection: signalR.HubConnection | null = null;

export const hubMessageReceived = createEvent<HubMessage>();

export const startSignalRFx = createEffect<void, void>(async () => {
    if (connection?.state === signalR.HubConnectionState.Connected) {
        return;
    }

    if (!connection) {
        connection = new signalR.HubConnectionBuilder()
            .withUrl(`https://localhost:10001/auctionHub`)
            .withAutomaticReconnect()
            .build();

        connection.on("Receive", (message: string) => {
            const parsed = parseHubMessage(message);
            if (parsed) {
                hubMessageReceived(parsed);
            }
        });
    }

    await connection.start();
});

sample({
    clock: initAuthFx.doneData,
    filter: Boolean,
    target: startSignalRFx,
});