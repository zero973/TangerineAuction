import { Container, Burger, Group, Anchor, ActionIcon, Image, Text, Button, Popover, Stack, Divider } from "@mantine/core";
import { useDisclosure } from "@mantine/hooks";
import { BsPerson } from "react-icons/bs";
import { IoExitOutline } from "react-icons/io5";
import ImageLogo from "./assets/image.png";
import { routes } from "@shared/routes";
import { Link } from "atomic-router-react";
import { $isAuth, $user, doLoginFx, doLogoutFx } from "@shared/api/auth/model";
import { useUnit } from "effector-react";

export const MainHeader = () => {
    const [opened, { toggle }] = useDisclosure();
    const [isAuth, userData] = useUnit([$isAuth, $user]);
    
    return (
        <>
            <Container h="100%">
                <Burger opened={opened} onClick={toggle} pos="absolute" hiddenFrom="sm" size="sm" />

                <Group h="100%" justify="space-between">
                    <Group h="100%" pos="relative" align="center">
                        <Image src={ImageLogo} mah="60%" onClick={() => routes.home.open()} />
                    </Group>

                    <Group>
                        <Anchor fz="h3" component={Link} to={routes.home} fw={window.location.pathname?.length === 1 ? 600 : 400}>
                            Главная
                        </Anchor>
                        {isAuth && userData?.isAdmin && (
                            <Anchor fz="h3" component={Link} to={routes.developer} fw={window.location.pathname?.length === 1 ? 400 : 600}>
                                Админка
                            </Anchor>
                        )}
                    </Group>

                    {isAuth ? (
                        <Group gap="0">
                            <Popover withArrow radius="lg">
                                <Popover.Target>
                                    <ActionIcon variant="outline">
                                        <BsPerson style={{ cursor: "pointer" }} />
                                    </ActionIcon>
                                </Popover.Target>

                                <Popover.Dropdown>
                                    <Stack>
                                        <Stack gap={4}>
                                            <Stack gap={0}>
                                                <Text fz="xs" c="dimmed">
                                                    Пользователь:
                                                </Text>

                                                <Text fz="md" fw={600}>
                                                    {userData?.firstName + " " + userData?.lastName}
                                                </Text>
                                            </Stack>
                                            <Stack gap={0}>
                                                <Text fz="xs" c="dimmed">
                                                    Email:
                                                </Text>

                                                <Text fz="md" fw={600}>
                                                    {userData?.email}
                                                </Text>
                                            </Stack>
                                        </Stack>

                                        <Divider />

                                        <Group justify="right">
                                            <Button
                                                color="red"
                                                size="sm"
                                                onClick={() => doLogoutFx()}
                                                rightSection={<IoExitOutline size="20px" />}
                                            >
                                                Выход
                                            </Button>
                                        </Group>
                                    </Stack>
                                </Popover.Dropdown>
                            </Popover>
                        </Group>
                    ) : (
                        <Button onClick={() => doLoginFx()} variant="outline" color="red">
                            Вход
                        </Button>
                    )}
                </Group>
            </Container>
        </>
    );
};