export enum HubMessageType {
    NewBetAdded = "NewBetAdded",
    AuctionAdded = "AuctionAdded",
    AuctionFinished = "AuctionFinished",
}

export type HubMessage = {
    type: HubMessageType;
    entityId: string;
};

export function parseHubMessage(message: string): HubMessage | null {
    const [type, ...rest] = message.split(":");
    const entityId = rest.join(":");

    if (!type || !entityId) return null;
    if (!Object.values(HubMessageType).includes(type as HubMessageType)) return null;

    return {
        type: type as HubMessageType,
        entityId,
    };
}