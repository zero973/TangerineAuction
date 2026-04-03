import { useEffect } from "react";
import {
    Button,
    Container,
    Group,
    Stack,
    Text,
    TextInput,
    Paper,
    Image,
    Grid,
    Select,
    Checkbox
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
} from "./model";
import { routes } from "@shared/routes";
import { $isAuth } from "@shared/api/auth/model.ts";
import { AuctionResponse } from "@shared/api/contracts/auction.ts";

export const HomePage = () => {

    type HomePageFormValues = {
        auctionName: string;
        tangerineName: string;
        tangerineQuality: number | null;
        showFinishedAuctions: boolean;
        isCurrentUserWinner: boolean;
    };
    
    const tangerineQualityLabels: Record<number, string> = {
        0: "Любое",
        1: "Common",
        2: "Uncommon",
        3: "Rare",
        4: "Legendary",
    };

    const [auctions, pagination, isAuth, loading] = useUnit([
        $auctions,
        $pagination,
        $isAuth,
        getAuctions.pending,
    ]);

    const form = useForm<HomePageFormValues>({
        initialValues: {
            auctionName: "",
            tangerineName: "",
            tangerineQuality: null,
            showFinishedAuctions: false,
            isCurrentUserWinner: false,
        },
    });

    const searchParams = {
        skip: 0,
        take: pagination.take,
        auctionName: form.values.auctionName || null,
        tangerineName: form.values.tangerineName || null,
        tangerineQuality: form.values.tangerineQuality ?? null,
        showFinishedAuctions: form.values.showFinishedAuctions,
        isCurrentUserWinner: form.values.isCurrentUserWinner,
    };

    useEffect(() => {
        searchAuctions(searchParams);
    }, [
        form.values.auctionName,
        form.values.tangerineName,
        form.values.tangerineQuality,
        form.values.showFinishedAuctions,
        form.values.isCurrentUserWinner,
        pagination.take,
    ]);

    const handleNext = () => {
        const nextSkip = pagination.skip + pagination.take;

        goNextPage();

        getAuctions({
            skip: nextSkip,
            take: pagination.take,
            auctionName: form.values.auctionName || null,
            tangerineName: form.values.tangerineName || null,
            tangerineQuality: form.values.tangerineQuality ?? null,
            showFinishedAuctions: form.values.showFinishedAuctions,
            isCurrentUserWinner: form.values.isCurrentUserWinner,
        });
    };

    const handlePrev = () => {
        const prevSkip = Math.max(0, pagination.skip - pagination.take);

        goPrevPage();

        getAuctions({
            skip: prevSkip,
            take: pagination.take,
            auctionName: form.values.auctionName || null,
            tangerineName: form.values.tangerineName || null,
            tangerineQuality: form.values.tangerineQuality ?? null,
            showFinishedAuctions: form.values.showFinishedAuctions,
            isCurrentUserWinner: form.values.isCurrentUserWinner,
        });
    };

    return (
        <Container>
            <Stack>
                <Group wrap="nowrap">
                    <Select
                        searchable
                        required
                        variant="filled"
                        data={Object.entries(tangerineQualityLabels).map(([value, label]) => ({
                            label,
                            value: value.toString(),
                        }))}
                        placeholder="Качество мандарина"
                        style={{ flex: "0 0 30%" }}
                        onChange={(value) => {
                            form.setFieldValue("tangerineQuality", value ? Number(value) : null);
                        }}
                    />

                    <TextInput
                        placeholder="Название аукциона..."
                        variant="filled"
                        w="100%"
                        value={form.values.auctionName}
                        style={{ flex: "1 1 70%" }}
                        onChange={(e) => {
                            form.setFieldValue("auctionName", e.target.value);
                        }}
                    />
                </Group>

                <TextInput
                    placeholder="Название мандарина..."
                    variant="filled"
                    w="100%"
                    value={form.values.tangerineName}
                    onChange={(e) => {
                        form.setFieldValue("tangerineName", e.target.value);
                    }}
                />
                <Group>
                    <Checkbox
                        label="Включить отображение завершённых аукционов"
                        onChange={(e) => {
                            form.setFieldValue("showFinishedAuctions", e.currentTarget.checked);
                        }}
                    />

                    { isAuth ? (
                        <Checkbox
                            label="Отображать аукционы, где я победитель"
                            onChange={(e) => {
                                form.setFieldValue("isCurrentUserWinner", e.currentTarget.checked);
                            }}
                        />
                        ) : "" }
                </Group>

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

                                            <Text
                                                size="sm"
                                                component="span"
                                                style={{
                                                    display: "inline-block",
                                                    backgroundColor: "#b0afae",
                                                    padding: "2px 8px",
                                                    borderRadius: 9999,
                                                    color: "#fff",
                                                    whiteSpace: "nowrap",
                                                }}
                                            >
                                                {auction.lastBet} ₽
                                            </Text>
                                        </div>
                                        
                                    </div>
                                </Grid.Col>

                                <Grid.Col span={12}>
                                    <Text size="sm" ta="left">
                                        Лот: {auction.tangerineName}
                                    </Text>
                                </Grid.Col>

                                <Grid.Col span={12}>
                                    <Text size="sm" ta="left">
                                        Качество: {tangerineQualityLabels[auction.tangerineQuality+1]}
                                    </Text>
                                </Grid.Col>

                                <Grid.Col span={12}>
                                    <Image
                                        src={auction.imageUrl}
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