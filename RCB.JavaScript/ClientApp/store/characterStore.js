import { createSlice } from "@reduxjs/toolkit";
import CharacterService from "@Services/CharacterService";

import _set from "lodash/set";
import _get from "lodash/get";
import _groupBy from "lodash/groupBy";
import _map from "lodash/map";

// Create the slice.
const slice = createSlice({
    name: "character",
    initialState: {
        isFetching: false,
        lastError: null,
        character: null
    },
    reducers: {
        setLastError: (state, action) => {
            const lastError = action.payload;
            state.lastError = lastError;
        },
        setFetching: (state, action) => {
            const isFetching = action.payload;
            state.isFetching = isFetching;
        },
        putData: (state, action) => {
            const character = action.payload;
            const socketedItemGroups = _get(character, "items", [])
                .filter(
                    (item) =>
                        item.inventoryId !== "PassiveJewels" &&
                        item.inventoryId !== "Flask" &&
                        item.inventoryId !== "Weapon2" &&
                        item.inventoryId !== "Offhand2"
                )
                .map((item) => {
                    const inventoryId = item.inventoryId;
                    // TODO
                    // may be move to server-side?
                    const _groupIndexs = _groupBy(
                        _get(item, "sockets", []).map((s, index) => {
                            return { ...s, index };
                        }),
                        (s) => s.group
                    );
                    const groups = _map(_groupIndexs, (gIndexs) => {
                        return gIndexs
                            .map((gi) => {
                                return _get(item, ["socketedItems", gi.index], null);
                            })
                            .filter((item) => item !== null);
                    })
                        .map((g) => {
                            return g.sort((a, b) => {
                                const aSupport = _get(a, "support", false) || _get(a, "typeLine") == "Spellslinger Support";
                                const bSupport = _get(b, "support", false) || _get(b, "typeLine") == "Spellslinger Support";
                                if (aSupport && bSupport) {
                                    return 0;
                                } else if (aSupport && !bSupport) {
                                    return 1;
                                } else if (!aSupport && bSupport) {
                                    return -1;
                                } else {
                                    return 0;
                                }
                            });
                        })
                        .sort((a, b) => b.length - a.length);
                    return {
                        inventoryId,
                        item,
                        groups
                    };
                })
                .filter((sg) => {
                    return sg.groups.length > 0;
                })
                .sort((a, b) => {
                    const aMaxLink = Math.max(...a.groups.map((g) => g.length));
                    const bMaxLink = Math.max(...b.groups.map((g) => g.length));
                    return bMaxLink - aMaxLink;
                });
            _set(character, "socketedItemGroups", socketedItemGroups);
            return {
                ...state,
                character
            };
        },
        clearData: (state) => {
            return { ...state, character: null };
        }
    }
});

// Export reducer from the slice.
export const { reducer } = slice;

// Define action creators.
export const actionCreators = {
    putCharacter: (accountName, name) => async (dispatch) => {
        dispatch(slice.actions.setFetching(true));

        const service = new CharacterService();

        const result = await service.getByName(accountName, name);

        if (!result.hasErrors) {
            dispatch(slice.actions.putData(result.value));
            dispatch(slice.actions.setLastError(null));
        } else {
            dispatch(slice.actions.setLastError(_get(result, "lastError", null)));
        }

        dispatch(slice.actions.setFetching(false));

        return result;
    },
    putCharacterData: (data) => (dispatch) => {
        dispatch(slice.actions.putData(data));
    },
    clear: () => (dispatch) => {
        dispatch(slice.actions.clearData());
    }
};
