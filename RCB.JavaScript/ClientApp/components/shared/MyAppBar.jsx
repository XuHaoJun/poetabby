import * as React from "react";

import { Link as RouterLink } from "react-router-dom";

import AppBar from "@material-ui/core/AppBar";
import Toolbar from "@material-ui/core/Toolbar";
import Button from "@material-ui/core/Button";
import IconButton from "@material-ui/core/IconButton";
import Typography from "@material-ui/core/Typography";
import LinearProgress from "@material-ui/core/LinearProgress";
import InfoIcon from "@material-ui/icons/Info";

import { toast } from "react-toastify";

import { small as logoImg } from "@Images/logos";

export default function MyAppBar(props) {
    const { isFetching = false } = props;
    return (
        <AppBar position="fixed">
            <div style={{ position: "relative" }}>
                {isFetching ? (
                    <LinearProgress
                        style={{ position: "absolute", top: 0, left: 0, width: "100%", height: 3 }}
                        color="secondary"
                    />
                ) : null}
                <Toolbar variant="dense">
                    <IconButton size="small" component={RouterLink} to="/">
                        <img src={logoImg} style={{ width: 40, height: 40 }} />
                    </IconButton>
                    <Button component={RouterLink} to="/" color="inherit" style={{ textTransform: "none", padding: 0 }}>
                        <Typography variant="h6">poe.tabby</Typography>
                    </Button>
                    <div style={{ flex: "1 1 auto" }} />
                    <IconButton
                        color="inherit"
                        onClick={() => {
                            toast.info(
                                <div>
                                    <h3>Dev Warning</h3>
                                    <h4><s>Usually take 5~15 sec delay over 1000+ characters if cache miss.</s></h4>
                                    <p>
                                        still in the development stage, not start optmize work,and will do optmize after extend
                                        POB for DPS show.
                                    </p>
                                    <p>
                                        <s>ecause server-side scan each char's items and passives and couting in realtime(sql COUNT
                                        really slow)</s>b,should save count result for each character and then sum count for filtered
                                        chars.may be put filtering and suming in client-side?(poe.ninja do)
                                    </p>
                                </div>
                            );
                        }}
                    >
                        <InfoIcon />
                    </IconButton>
                </Toolbar>
            </div>
        </AppBar>
    );
}
