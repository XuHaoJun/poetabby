import * as loginStore from "@Store/loginStore";
import * as personStore from "@Store/personStore";
import * as leagueStore from "@Store/leagueStore";
import * as characterStore from "@Store/characterStore";
import * as serverEnvStore from "@Store/serverEnvStore";

// Whenever an action is dispatched, Redux will update each top-level application state property using
// the reducer with the matching name. It's important that the names match exactly, and that the reducer
// acts on the corresponding ApplicationState property type.
export const reducers = {
    login: loginStore.reducer,
    person: personStore.reducer,
    league: leagueStore.reducer,
    character: characterStore.reducer,
    serverEnv: serverEnvStore.reducer
};
