import { useEffect, useMemo } from "react";
import {
    Container,
    Group,
    Stack,
    Text,
    Paper,
    Image,
    Button,
    Grid,
    NumberInput
} from "@mantine/core";
import { useForm } from "@mantine/form";
import { useUnit } from "effector-react";
import { BsCashCoin, BsCart2 } from "react-icons/bs";
import { showError, showSuccess } from "@shared/notifications.tsx";
import { $auction, $canCreateBetResult, addBet, buyTangerine } from "@pages/auction/model.ts";
import { $user } from "@shared/api/auth/model.ts";

export const AuctionPage = () => {

    const tangerineQualityLabels: Record<number, string> = {
        0: "Common",
        1: "Uncommon",
        2: "Rare",
        3: "Legendary",
    };
    
    const [auction, canBet, user] = useUnit([$auction, $canCreateBetResult, $user]);
    
    const nextBetAllowedValue = useMemo(() => {
        if (!auction) return 10;
        return auction.bets.at(-1)!.price + 1;
    }, [auction]);

    const betForm = useForm({
        initialValues: {
            auctionId: "",
            price: nextBetAllowedValue,
        },
    });

    useEffect(() => {
        if (!auction) return;

        betForm.setValues({
            auctionId: auction.auctionId,
            price: nextBetAllowedValue,
        });
    }, [auction, nextBetAllowedValue]);

    const handleAddBet = async (v: typeof betForm.values) => {
        try {
            const valid = betForm.validate();
            if (valid.hasErrors) {
                return;
            }

            const created = await addBet(v);

            showSuccess(`Ваша ставка ${created.price} зафиксирована. Ожидайте завершения аукциона`);
        } catch (err: any) {
            showError({
                title: err.response.data.title,
                message: err.response.data.detail,
            });
        }
    };

    const handleBuyTangerine = async () => {
        try {
            await buyTangerine({ id: auction!.auctionId });
            showSuccess(`Вы успешно выкупили лот`);
        } catch (err: any) {
            showError({
                title: err.response.data.title,
                message: err.response.data.detail,
            });
        }
    };

    if (!auction) {
        return (
            <Container>
                <Text>Loading...</Text>
            </Container>
        );
    }
    
    return (
        <Container>
            <Paper radius="lg" p="md" withBorder>
                <Grid gutter="xs">
                    <Grid.Col span={12}>
                        <div
                            style={{
                                display: "grid",
                                gridTemplateColumns: "1fr auto 1fr",
                                alignItems: "center",
                                columnGap: "16px",
                                marginBottom: 16,
                            }}
                        >
                            <Text size="sm" c="dimmed" ta="left" truncate>
                                {new Date(auction!.createdOn).toLocaleString()}
                            </Text>

                            <Text fw={600} size="lg" ta="center" truncate>
                                {auction!.name}
                            </Text>

                            <div
                                style={{
                                    display: "flex",
                                    justifyContent: "flex-end",
                                    gap: "8px",
                                    flexWrap: "nowrap",
                                    whiteSpace: "nowrap",
                                }}
                            >
                                {!auction.isActual && (
                                    <Text
                                        size="sm"
                                        component="span"
                                        style={{
                                            display: "inline-block",
                                            backgroundColor: "#db73eb",
                                            padding: "2px 8px",
                                            borderRadius: 9999,
                                            color: "#fff",
                                            whiteSpace: "nowrap",
                                        }}
                                    >
                                        Завершён
                                    </Text>
                                )}
                            </div>
                        </div>
                    </Grid.Col>

                    <Grid.Col span={7}>
                        <Stack mt="md">
                            <Text size="lg" ta="left">
                                Лот: {auction!.tangerine.name}
                            </Text>

                            <Text size="lg" ta="left">
                                Качество: {tangerineQualityLabels[auction!.tangerine.quality]}
                            </Text>

                            <Text size="lg" ta="left">
                                Цена выкупа: {auction!.tangerine.buyPrice} ₽
                            </Text>

                            <Image
                                src={auction!.tangerine.imageUrl}
                                fit="contain"
                                radius="md"
                                fallbackSrc="https://placehold.co/600x400?text=Placeholder"
                            />
                        </Stack>
                    </Grid.Col>

                    <Grid.Col span={5} >
                        <Stack gap="xs" mt="md" ml="md">
                            <Text size="lg" ta="center">
                                Ставки
                            </Text>
                            {auction.bets.length > 0 ? (
                                auction.bets
                                    .slice()
                                    .reverse()
                                    .map((bet) => (
                                        <Paper key={bet.id} withBorder p="xs" radius="md">
                                            <Group justify="space-between" gap="xs">
                                                <Text fw={600} size="sm">
                                                    {bet.price.toLocaleString()} ₽
                                                </Text>

                                                {
                                                    bet.createdBy === user?.id ? (
                                                        <Text size="sm">(ваша)</Text>
                                                    ) : ""
                                                }
                                                
                                                <Text size="sm" c="dimmed">
                                                    {new Date(bet.createdOn).toLocaleString()}
                                                </Text>
                                            </Group>
                                        </Paper>
                                    ))
                            ) : (
                                <Text size="sm" c="dimmed">
                                    Ставок пока нет
                                </Text>
                            )}
                        </Stack>
                        { canBet ? (
                            <Group mt="md" ml="md">
                                <NumberInput label="Введите вашу ставку"
                                             placeholder="100"
                                             error={`Значение должно быть от ${nextBetAllowedValue} до 1 млн.`}
                                             min={nextBetAllowedValue}
                                             max={999_999}
                                             {...betForm.getInputProps("price")} />

                                <Button
                                    variant="outline"
                                    onClick={() => handleAddBet(betForm.values)}
                                    h={42}
                                    px="md"
                                    leftSection={<BsCashCoin />}
                                >
                                    Сделать ставку
                                </Button>

                                <Button
                                    variant="outline"
                                    onClick={() => handleBuyTangerine()}
                                    h={42}
                                    px="md"
                                    leftSection={<BsCart2 />}
                                >
                                    Выкупить лот
                                </Button>
                            </Group>
                        ) : "" }
                    </Grid.Col>
                </Grid>
            </Paper>
        </Container>
    );
};