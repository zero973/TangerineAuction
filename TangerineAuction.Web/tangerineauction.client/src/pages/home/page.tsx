import {
    Button,
    Container,
    Group,
    Stack,
    Text,
    TextInput,
    Paper,
    Image,
    Grid
} from "@mantine/core";
import { useForm } from "@mantine/form";
import { useUnit } from "effector-react";
import {
    $auctions,
    searchAuctions,
    goNextPage,
    goPrevPage,
    $pagination,
    getAuctions, 
    AuctionResponse,
} from "./model";
import { routes } from "@shared/routes";

export const HomePage = () => {
    
    const tangerineQualityLabels: Record<number, string> = {
        0: "Common",
        1: "Uncommon",
        2: "Rare",
        3: "Legendary",
    };

    const [auctions, pagination, loading] = useUnit([
        $auctions,
        $pagination,
        getAuctions.pending,
    ]);

    const form = useForm({
        initialValues: {
            name: "",
        },
    });

    // Когда меняется name — триггерим поиск (debounce будет в model)
    const onSearchChange = (value: string) => {
        searchAuctions({
            name: value || null,
            skip: 0,
            take: pagination.take,
        });
    };

    const handleNext = () => {
        goNextPage();
        const nextSkip = pagination.skip + pagination.take;
        getAuctions({ skip: nextSkip, take: pagination.take, name: form.values.name || null });
    };
    const handlePrev = () => {
        const prevSkip = Math.max(0, pagination.skip - pagination.take);
        goPrevPage();
        getAuctions({ skip: prevSkip, take: pagination.take, name: form.values.name || null });
    };

    return (
        <Container>
            <Stack>
                <TextInput
                    placeholder="Название аукциона..."
                    variant="filled"
                    w="100%"
                    value={form.values.name}
                    onChange={(e) => {
                        form.setFieldValue("name", e.target.value);
                        onSearchChange(e.target.value);
                    }}
                />

                <Stack align="stretch" justify="flex-start" gap="sm">
                    {loading && <Text>Loading...</Text>}

                    {auctions?.map((auction: AuctionResponse) => (
                        <Paper
                            key={auction.auctionId}
                            onClick={() => routes.auction.open({ id: auction.auctionId })}
                            radius="lg"
                            p="md"
                            withBorder
                            style={{ cursor: "pointer" }}
                        >
                            <Grid gutter="xs">
                                <Grid.Col span={12}>
                                    <div
                                        style={{
                                            display: "grid",
                                            gridTemplateColumns: "1fr auto 1fr",
                                            alignItems: "center",
                                            columnGap: "16px",
                                        }}
                                    >
                                        <Text size="sm" c="dimmed" ta="left" truncate>
                                            {new Date(auction.auctionCreatedOn).toLocaleString()}
                                        </Text>

                                        <Text fw={600} size="lg" ta="center" truncate>
                                            {auction.auctionName}
                                        </Text>

                                        <Text size="sm" ta="right" truncate>
                                            {auction.lastBet} ₽
                                        </Text>
                                    </div>
                                </Grid.Col>

                                <Grid.Col span={12}>
                                    <Text size="sm" ta="left">
                                        Лот: {auction.tangerineName}
                                    </Text>
                                </Grid.Col>

                                <Grid.Col span={12}>
                                    <Text size="sm" ta="left">
                                        Качество: {tangerineQualityLabels[auction.tangerineQuality]}
                                    </Text>
                                </Grid.Col>

                                <Grid.Col span={12}>
                                    <Image
                                        src={`https://localhost:10001/images/${auction.filePath}`}
                                        fit="contain"
                                        radius="md"
                                        fallbackSrc="https://placehold.co/600x400?text=Placeholder"
                                    />
                                </Grid.Col>
                            </Grid>
                        </Paper>
                    ))}
                </Stack>

                <Group justify="center">
                    <Button onClick={handlePrev} disabled={pagination.skip === 0}>
                        Prev
                    </Button>
                    <Text>
                        {pagination.skip + 1} - {pagination.skip + auctions.length}
                    </Text>
                    <Button onClick={handleNext} disabled={auctions.length < pagination.take}>
                        Next
                    </Button>
                </Group>
            </Stack>
        </Container>
    );
};