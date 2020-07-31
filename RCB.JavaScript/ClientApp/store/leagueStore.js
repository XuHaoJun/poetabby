import { createSlice } from "@reduxjs/toolkit";
import _set from "lodash/set";
import _get from "lodash/get";

import { isNode } from "@Utils";
import LeagueService from "@Services/LeagueService";
import poeClasses from "../constans/poeClasses";

// Create the slice.
const slice = createSlice({
    name: "league",
    initialState: {
        isFetching: false,
        delayClearTimer: null,
        defaultLeagueName: "Harvest",
        leagueName: "Harvest",
        total: 0,
        entries: [],
        analysis: {}
    },
    reducers: {
        setFetching: (state, action) => {
            state.isFetching = action.payload;
        },
        setLeagueName: (state, action) => {
            state.leagueName = action.payload;
        },
        setDelayClearTimer: (state, action) => {
            const oldTimer = state.delayClearTimer;
            const nextTimer = action.payload;
            if ((nextTimer && oldTimer) || (nextTimer === null && oldTimer)) {
                clearTimeout(oldTimer);
            }
            if (oldTimer !== nextTimer) {
                state.delayClearTimer = nextTimer;
            }
        },
        putData: (state, action) => {
            const { total, entries, ...analysis } = action.payload;
            const classCountEntries = _get(analysis, "classCountEntries", []);
            const minNumClass = 19;
            if (classCountEntries.length < minNumClass) {
                const injectClasses = poeClasses.ascendancy
                    .filter((asceClass) => {
                        const found = classCountEntries.some((entry) => {
                            return entry.class === asceClass;
                        });
                        return !found;
                    })
                    .map((c) => {
                        return { class: c, count: 0 };
                    });
                _set(analysis, "classCountEntries", classCountEntries.concat(injectClasses));
            }
            return {
                ...state,
                total,
                entries,
                analysis
            };
        },
        clearData: (state) => {
            return { ...state, total: 0, entries: [], analysis: {} };
        }
    }
});

// Export reducer from the slice.
export const { reducer } = slice;

// Define action creators.
// TODO
// Use timestamp instead of setTimeout.
export const actionCreators = {
    putCharacters: (leagueName, config) => async (dispatch) => {
        dispatch(slice.actions.setDelayClearTimer(null));
        dispatch(slice.actions.setFetching(true));

        const service = new LeagueService();

        const result = await service.getCharacters(leagueName, config);

        if (!result.hasErrors) {
            dispatch(slice.actions.putData(result.value));
        } else if (_get(result.lastError, "response.status") === 404) {
            dispatch(slice.actions.putData({ total: 0, entries: [], analysis: {} }));
        }

        dispatch(slice.actions.setFetching(false));

        return result;
    },
    cancelDelayClear: () => (dispatch) => {
        dispatch(slice.actions.setDelayClearTimer(null));
    },
    delayClear: (delay = 60000) => (dispatch) => {
        if (isNode()) {
            dispatch(slice.actions.clearData());
        } else {
            const timer = setTimeout(() => {
                dispatch(slice.actions.clearData());
            }, delay);
            dispatch(slice.actions.setDelayClearTimer(timer));
        }
    },
    clear: () => (dispatch) => {
        dispatch(slice.actions.clearData());
    }
    // TODO add cancleAllRequests action for page unmount.
};
