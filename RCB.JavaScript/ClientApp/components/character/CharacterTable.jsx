import React from "react";
import queryString from "query-string";
import reactEqual from "fast-deep-equal/react";

import _cloneDeep from "lodash/cloneDeep";
import _get from "lodash/get";
import _keys from "lodash/keys";

import { makeStyles, withStyles } from "@material-ui/core/styles";
import Table from "@material-ui/core/Table";
import TableBody from "@material-ui/core/TableBody";
import TableCell from "@material-ui/core/TableCell";
import TableContainer from "@material-ui/core/TableContainer";
import TableHead from "@material-ui/core/TableHead";
import TableRow from "@material-ui/core/TableRow";
import TableSortLabel from "@material-ui/core/TableSortLabel";

import Paper from "@material-ui/core/Paper";

import { Link as RouterLink } from "react-router-dom";
import Button from "@material-ui/core/Button";
import IconButton from "@material-ui/core/IconButton";
import SearchIcon from "@material-ui/icons/Search";

import * as PoeAvatars from "@Images/poeAvatars";
import treeData from "../../treeData";

import Tooltip from "@material-ui/core/Tooltip";
import LazyTooltip from "@Components/shared/LazyTooltip";

import Grid from "@material-ui/core/Grid";

const allKeystones = treeData.getKeystones();

const BootstrapTooltip = withStyles((theme) => ({
    arrow: {
        color: theme.palette.common.black
    },
    tooltip: {
        backgroundColor: theme.palette.common.black
    }
}))(function BootstrapTooltip(props) {
    const classes = props.classes;
    return <LazyTooltip arrow classes={classes} {...props} />;
});

function reverseArrange(arrange) {
    if (arrange === "desc") {
        return "asc";
    } else {
        return "desc";
    }
}

export function CharacterTable(props) {
    const data = props.data || [];
    const isFetching = _get(props, "isFetching", false);
    const backBuildsPageUrl = props.backBuildsPageUrl;
    const defaultOrderBy = ["level", "desc"];
    const orderBy = props.orderBy || defaultOrderBy;
    const orderByField = _get(orderBy, 0, "level");
    const orderByArrange = _get(orderBy, 1, "desc");
    const config = _get(props, "config", {});
    function getNextConfig(fieldName) {
        const selected = fieldName === orderByField;
        var nextConfig;
        if (selected) {
            nextConfig = { ..._cloneDeep(config), orderBy: [fieldName, reverseArrange(orderByArrange)] };
        } else {
            nextConfig = { ..._cloneDeep(config), orderBy: [fieldName, "desc"] };
        }
        if (nextConfig.orderBy[0] === "level" && nextConfig.orderBy[1] === "desc") {
            delete nextConfig.orderBy;
        }
        delete nextConfig.offset;
        return nextConfig;
    }
    function getNextTo(fieldName) {
        if (isFetching) {
            return "#";
        } else {
            const nextConfig = getNextConfig(fieldName);
            return queryString.stringifyUrl({ url: "/", query: nextConfig });
        }
    }
    return (
        <Table aria-label="simple table" size="small">
            <TableHead>
                <TableRow>
                    <TableCell align="center" padding="none">
                        Name
                    </TableCell>
                    <TableCell align="center">
                        <TableSortLabel
                            active={orderByField === "level"}
                            direction={orderByField === "level" ? orderByArrange : "desc"}
                            disabled={isFetching}
                            component={RouterLink}
                            to={getNextTo("level")}
                        >
                            Level
                        </TableSortLabel>
                    </TableCell>
                    <TableCell align="right">
                        <TableSortLabel
                            active={orderByField === "life"}
                            direction={orderByField === "life" ? orderByArrange : "desc"}
                            disabled={isFetching}
                            component={RouterLink}
                            to={getNextTo("life")}
                        >
                            Life
                        </TableSortLabel>
                    </TableCell>
                    <TableCell align="right">
                        <TableSortLabel
                            active={orderByField === "es"}
                            direction={orderByField === "es" ? orderByArrange : "desc"}
                            disabled={isFetching}
                            component={RouterLink}
                            to={getNextTo("es")}
                        >
                            ES
                        </TableSortLabel>
                    </TableCell>
                    <TableCell align="right">Depth</TableCell>
                    <TableCell align="center">DPS</TableCell>
                    <TableCell align="center" padding="none">
                        Skill&nbsp;/&nbsp;Keystones
                    </TableCell>
                </TableRow>
            </TableHead>
            <TableBody>
                {data.map((pchar) => {
                    const keystones = pchar.treeNodes
                        .split(",")
                        .filter((skillId) => {
                            return allKeystones.some((keystone) => {
                                return `${keystone.skill}` == skillId;
                            });
                        })
                        .map((skillId) => treeData.getNode(skillId));
                    return (
                        <TableRow key={pchar.characterId}>
                            <TableCell
                                align="left"
                                style={{
                                    padding: 0,
                                    maxWidth: 170,
                                    width: 170
                                }}
                            >
                                <Tooltip placement="top" title={pchar.characterName} enterDelay={1000} enterNextDelay={1000}>
                                    <Button
                                        style={{
                                            textTransform: "none"
                                        }}
                                        disabled={isFetching}
                                        size="small"
                                        variant="outlined"
                                        color="primary"
                                        fullWidth
                                        component={RouterLink}
                                        to={
                                            isFetching
                                                ? "#"
                                                : {
                                                      pathname: `/characters/${pchar.accountName}/${pchar.characterName}`,
                                                      state: backBuildsPageUrl
                                                          ? {
                                                                backBuildsPageUrl,
                                                                from: props.fromLocation
                                                            }
                                                          : undefined
                                                  }
                                        }
                                        onClick={(e) => {
                                            if (pchar && props.putCharacterData) {
                                                props.putCharacterData(pchar);
                                            }
                                        }}
                                    >
                                        <span
                                            style={{
                                                overflow: "hidden",
                                                textOverflow: "ellipsis",
                                                whiteSpace: "nowrap"
                                            }}
                                        >
                                            {pchar.characterName}
                                        </span>
                                    </Button>
                                </Tooltip>
                            </TableCell>
                            <TableCell align="right">
                                <div
                                    style={{
                                        display: "flex",
                                        justifyContent: "center",
                                        alignItems: "center",
                                        height: "100%",
                                        width: "100%"
                                    }}
                                >
                                    <span>100&nbsp;</span>
                                    <img src={PoeAvatars[pchar.class]} style={{ width: 50, height: 40 }} />
                                </div>
                            </TableCell>
                            <TableCell align="right">{_get(pchar, "lifeUnreserved", 0)}</TableCell>
                            <TableCell align="right">{_get(pchar, "energyShield", 0)}</TableCell>
                            <TableCell align="right">{pchar.depth.default}</TableCell>
                            <TableCell align="center"></TableCell>
                            <TableCell align="center">
                                <div style={{ display: "flex", justifyContent: "space-between" }}>
                                    <div>
                                        <img
                                            src="//web.poecdn.com/image/Art/2DItems/Gems/Portal.png?v=5e96a91147fde1aad67e09a24a93d6e8&w=1&h=1&scale=1"
                                            style={{ width: 40, height: 40 }}
                                        />
                                    </div>
                                    <BootstrapTooltip
                                        placement="left-start"
                                        arrow
                                        renderTitle={() => (
                                            <React.Fragment>
                                                {keystones.map((keystone) => {
                                                    return (
                                                        <Grid container key={keystone.skill} alignItems="center">
                                                            <Grid item>
                                                                <img
                                                                    src={`//web.poecdn.com/image/${keystone.icon}`}
                                                                    style={{ width: 40, height: 40 }}
                                                                />
                                                            </Grid>
                                                            <Grid item>&nbsp;{keystone.name}</Grid>
                                                        </Grid>
                                                    );
                                                })}
                                            </React.Fragment>
                                        )}
                                    >
                                        <Grid container justify="center" alignItems="center">
                                            {keystones.slice(0, keystones.length <= 3 ? 3 : 2).map((keystone) => {
                                                return (
                                                    <Grid item key={keystone.skill}>
                                                        <img
                                                            src={`//web.poecdn.com/image/${keystone.icon}`}
                                                            style={{ width: 40, height: 40 }}
                                                        />
                                                    </Grid>
                                                );
                                            })}
                                            {keystones.length > 3 ? (
                                                <Grid item style={{ width: 40 }}>
                                                    +{keystones.length - 2}
                                                </Grid>
                                            ) : null}
                                        </Grid>
                                    </BootstrapTooltip>
                                </div>
                            </TableCell>
                        </TableRow>
                    );
                })}
            </TableBody>
        </Table>
    );
}

export default React.memo(CharacterTable, (a, b) => {
    const { data: _aData = [], ...aOthers } = a || {};
    const { data: _bData = [], ...bOthers } = b || {};
    const normalize = (x) => ({ id: x.characterId, updatedAt: x.updatedAt, keysLength: _keys(x).length });
    const aData = _aData.map(normalize);
    const bData = _bData.map(normalize);
    return reactEqual(aOthers, bOthers) && reactEqual(aData, bData);
});
