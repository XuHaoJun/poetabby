import * as React from "react";
import GuestLayout from "@Layouts/GuestLayout";
// import AuthorizedLayout from "@Layouts/AuthorizedLayout";
import LoginPage from "@Pages/LoginPage";
import AppRoute from "@Components/shared/AppRoute";
// import HomePage from "@Pages/HomePage";
// import ExamplesPage from "@Pages/ExamplesPage";
import { Switch } from "react-router-dom";
import PassiveTreePage from "@Pages/PassiveTreePage";
import BuildsPage from "@Pages/BuildsPage";
import CharacterPage from "@Pages/CharacterPage";
import CssbaseLayout from "@Layouts/CssbaseLayout";
import NotFoundPage from "@Pages/NotFoundPage";

export const routes = (
    <Switch>
        <AppRoute layout={CssbaseLayout} component={CharacterPage} exact path="/characters/:accountName/:characterName" />
        <AppRoute layout={CssbaseLayout} component={PassiveTreePage} exact path="/tree" />
        <AppRoute layout={CssbaseLayout} component={BuildsPage} exact path="/" />
        <AppRoute layout={GuestLayout} component={NotFoundPage} statusCode={404} />
    </Switch>
);
