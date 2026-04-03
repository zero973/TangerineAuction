export enum TangerineQuality {
    Common = 0,
    Uncommon = 1,
    Rare = 2,
    Legendary = 3,
}

export type TangerineResponse = {
    id: string;
    name: string;
    quality: TangerineQuality;
    startPrice: number;
    buyPrice: number;
    imageUrl: string;
    createdOn: string;
};