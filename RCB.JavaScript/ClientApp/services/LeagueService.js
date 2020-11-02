import { ServiceBase } from "@Core/ServiceBase";
import queryString from "query-string";

export default class LeagueService extends ServiceBase {
    async getDefaultLeague() {
        return await this.requestJson({
            url: "/api/leagues/defaultLeague",
            method: "GET"
        });
    }

    async getLeagues() {
        return await this.requestJson({
            url: "/api/leagues",
            method: "GET"
        });
    }

    async getCharacters(
        leagueName = "Harvest",
        config = {
            item: [],
            class: [],
            mainSkill: [],
            allSkill: [],
            keystone: [],
            weaponType: undefined,
            offhandType: undefined,
            characterNameLike: undefined,
            orderBy: undefined,
            limit: 50,
            offset: 0
        }
    ) {
        return await this.requestJson({
            url: queryString.stringifyUrl({ url: `/api/leagues/${leagueName}/characters`, query: config }),
            method: "GET"
        });
    }
}
