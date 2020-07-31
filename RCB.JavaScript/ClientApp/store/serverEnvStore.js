import { createSlice } from "@reduxjs/toolkit";
import { isNode } from "@Utils";

// Create the slice.
const slice = createSlice({
    name: "character",
    initialState: {
        readyFirstRenderOnServer: false
    },
    reducers: {
        setReadyFirstRenderOnServer: (state, action) => {
            if (isNode()) {
                state.readyFirstRenderOnServer = action.payload;
            } else {
                state.readyFirstRenderOnServer = false;
            }
        }
    }
});

// Export reducer from the slice.
export const { reducer } = slice;

// Define action creators.
export const actionCreators = {
    setReadyFirstRenderOnServer: (v) => {
        return slice.actions.setReadyFirstRenderOnServer(v);
    }
};
