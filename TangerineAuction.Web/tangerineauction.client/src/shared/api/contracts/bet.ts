export type BetResponse = {
    id: string;
    price: number;
    createdOn: string;
    createdBy: string;
};

export type AddBetBody = {
    auctionId: string;
    price: number;
};