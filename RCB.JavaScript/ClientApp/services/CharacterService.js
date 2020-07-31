import { ServiceBase } from "@Core/ServiceBase";
import queryString from "query-string";

export default class CharacterService extends ServiceBase {
  async getById(id) {
    var result = await this.requestJson({
      url: `/api/characters/${id}`,
      method: "GET",
    });
    return result;
  }

  async getByName(accountName, name) {
    var result = await this.requestJson({
      url: `/api/characters/${encodeURI(accountName)}/${encodeURI(name)}`,
      method: "GET",
    });
    return result;
  }
}