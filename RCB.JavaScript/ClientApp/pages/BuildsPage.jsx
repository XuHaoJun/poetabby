import * as React from "react";
import { Helmet } from "react-helmet";
import queryString from "query-string";

import reactEqual from "fast-deep-equal/react";
import _set from "lodash/set";
import _get from "lodash/get";
import _hasIn from "lodash/hasIn";
import _cloneDeep from "lodash/cloneDeep";
import _castArray from "lodash/castArray";
import _zipWith from "lodash/zipWith";
import _tail from "lodash/tail";
import _isString from "lodash/isString";

import * as LeagueStore from "@Store/leagueStore";
import * as CharacterStore from "@Store/characterStore";
import { connect } from "react-redux";
import { wait } from "domain-wait";

import { withRouter } from "react-router";
import { Link as RouterLink } from "react-router-dom";

import { makeStyles, withStyles } from "@material-ui/core/styles";
import Container from "@material-ui/core/Container";

import Paper from "@material-ui/core/Paper";

import Skeleton from "@material-ui/lab/Skeleton";

import AppBar from "@material-ui/core/AppBar";
import Toolbar from "@material-ui/core/Toolbar";
import Typography from "@material-ui/core/Typography";
import Button from "@material-ui/core/Button";
import IconButton from "@material-ui/core/IconButton";
import Tooltip from "@material-ui/core/Tooltip";
import MenuIcon from "@material-ui/icons/Menu";
import CloseIcon from "@material-ui/icons/Close";

import Avatar from "@material-ui/core/Avatar";
import Badge from "@material-ui/core/Badge";
import * as PoeAvatars from "@Images/poeAvatars";

import Grid from "@material-ui/core/Grid";

import List from "@material-ui/core/List";
import ListItem from "@material-ui/core/ListItem";
import ListItemSecondaryAction from "@material-ui/core/ListItemSecondaryAction";
import ListItemText from "@material-ui/core/ListItemText";
import ListItemAvatar from "@material-ui/core/ListItemAvatar";

import FormGroup from "@material-ui/core/FormGroup";
import FormLabel from "@material-ui/core/FormLabel";
import FormControlLabel from "@material-ui/core/FormControlLabel";
import Checkbox from "@material-ui/core/Checkbox";

import CancelIcon from "@material-ui/icons/Cancel";
import Chip from "@material-ui/core/Chip";

import CharacterTable from "@Components/character/CharacterTable";

import MyAppBar from "@Components/shared/MyAppBar";

import FlipMove from "react-flip-move";
import { isNode } from "domain-wait/dist/esm/src/utils";

import poeClasses from "../constans/poeClasses";
import treeData from "../treeData";

import { FixedSizeList } from "react-window";
import AutoSizer from "react-virtualized-auto-sizer";

import TextField from "@material-ui/core/TextField";
import Fuse from "fuse.js";
import { useDebounce } from "use-debounce";

import LazyTooltip from "@Components/shared/LazyTooltip";

import Pagination from "@material-ui/lab/Pagination";
import PaginationItem from "@material-ui/lab/PaginationItem";

import LeaguesMenu from "@Components/league/LeaguesMenu";

import NoSsr from "@material-ui/core/NoSsr";

import Sticky from "react-sticky-el";

import EnhancedChip from "@Components/shared/EnhancedChip";

const StyledBadge = withStyles((theme) => ({
    badge: {
        right: 7,
        bottom: 5,
        border: `2px solid ${theme.palette.background.paper}`,
        padding: "0 2px"
    }
}))(Badge);

const StyledListItem = withStyles((theme) => ({
    root: {
        transition: "background 1s ease-in-out",
        "&:hover": {
            border: "2px solid black"
        }
    },
    selected: {
        "&$selected, &$selected:hover": {
            backgroundColor: theme.palette.secondary.main
        },
        "&$selected:hover": {
            backgroundColor: theme.palette.secondary.dark
        }
    }
}))(ListItem);

function _FilterList(props) {
    const {
        isFetching = false,
        entries = [],
        total,
        config = {},
        configField,
        getConfigValue: _getConfigValue,
        getCount: _getCount,
        getDisplay: _getDisplay,
        configType = "scalar",
        renderTooltip,
        pathname = "/",
        ...otherProps
    } = props;
    const getCount =
        _getCount ||
        function (item) {
            return item.count;
        };
    const getConfigValue =
        _getConfigValue ||
        function (item) {
            return item.skillId || item.itemName;
        };
    const getDisplay =
        _getDisplay ||
        function (item) {
            if (configType === "weapon/offhand") {
                const getNormalized = (str) => {
                    return str.replace(/((^One Handed (.+)$)|(^Rune (Dagger)$))/, (match, p1, p2, p3, p4, p5) => {
                        return p3 || p5 || match;
                    });
                };
                const weaponType = getNormalized(item.weaponType);
                const offhandType = getNormalized(item.offhandType);
                return `${weaponType} / ${offhandType}`;
            } else {
                return item.nameSpec || item.itemName || _get(treeData.getNode(item.skillId), "name");
            }
        };
    function renderRow({ style = {}, index }) {
        const item = entries[index];
        const count = getCount(item, index, entries, config);
        const percentValue = (count / total) * 100;
        var selected;
        const getNextConfig = () => {
            if (configType === "array") {
                const configValue = getConfigValue(item);
                const configArray = _get(config, configField, []);
                var next;
                selected = configArray.includes(configValue);
                if (selected) {
                    const s = new Set(configArray);
                    s.delete(configValue);
                    next = [...s];
                } else {
                    const s = new Set(configArray);
                    s.add(configValue);
                    next = [...s];
                }
                return {
                    ..._cloneDeep(config),
                    [configField]: next
                };
            } else if (configType === "mainSkillSupportArray") {
                const configValue = getConfigValue(item);
                const configArrayIndex = _get(config, configField, []).findIndex((xs) => _get(xs, 0) === item.mainSkillId);
                const configArray = configArrayIndex != -1 ? _get(config, [configField, configArrayIndex]) : [];
                var next;
                selected = configArray.includes(configValue);
                if (selected) {
                    const s = new Set(_tail(configArray));
                    s.delete(configValue);
                    next = [item.mainSkillId, ...s];
                } else {
                    const s = new Set(_tail(configArray));
                    s.add(configValue);
                    next = [item.mainSkillId, ...s];
                }
                const cloned = _cloneDeep(config);
                if (configArrayIndex != -1) {
                    _set(cloned, [configField, configArrayIndex], next);
                } else {
                    _set(cloned, configField, _get(cloned, configField, []).concat([next]));
                }
                return cloned;
            } else if (configType === "weapon/offhand") {
                var nextConfig;
                for (const configField of ["weaponType", "offhandType"]) {
                    const configValue = getConfigValue(item, configField);
                    const configArray = _get(config, configField, []);
                    var next;
                    selected = configArray.includes(configValue);
                    if (selected) {
                        const s = new Set(configArray);
                        s.delete(configValue);
                        next = [...s];
                    } else {
                        const s = new Set(configArray);
                        s.add(configValue);
                        next = [...s];
                    }
                    nextConfig = {
                        ..._cloneDeep(nextConfig || config),
                        [configField]: next
                    };
                }
                return nextConfig;
            } else {
                const configValue = getConfigValue(item);
                selected = _get(config, configField, null) === configValue;
                return {
                    ..._cloneDeep(config),
                    [configField]: configValue
                };
            }
        };
        const nextConfig = getNextConfig();
        const to = isFetching
            ? "#"
            : queryString.stringifyUrl({
                  url: pathname,
                  query: nextConfig
              });
        const primary = (
            <Typography
                style={{
                    display: "flex",
                    justifyContent: "space-between",
                    width: "100%",
                    color: selected ? "#fff" : undefined
                }}
            >
                <span>{getDisplay(item, index, entries)}</span>
                <span>{Math.round(percentValue) === 0 ? percentValue.toFixed(1) : Math.round(percentValue)}%</span>
            </Typography>
        );
        const finalStyle = {
            ...style,
            background: selected
                ? undefined
                : `linear-gradient(to left, rgba(63, 81, 181, 0.5) ${percentValue.toFixed(
                      3
                  )}%, rgb(255, 255, 255) ${percentValue.toFixed(3)}%)`,
            userSelect: "none"
        };
        const listItem = (
            <StyledListItem
                button
                style={finalStyle}
                disabled={isFetching}
                selected={selected}
                dense
                component={RouterLink}
                to={to}
                replace
            >
                <ListItemText primary={primary} disableTypography />
            </StyledListItem>
        );
        if (renderTooltip) {
            return renderTooltip(item, listItem, index);
        } else {
            return listItem;
        }
    }
    const itemSize = 40;
    if (isNode()) {
        return (
            <FixedSizeList height={400} width="100%" itemSize={itemSize} itemCount={entries.length} {...otherProps}>
                {renderRow}
            </FixedSizeList>
        );
    } else if (entries.length === 0 && isFetching) {
        return (
            <List dense>
                {[...Array(9).keys()].map((i) => (
                    <ListItem key={i}>
                        <Skeleton variant="rect" width="100%" height={30} />
                    </ListItem>
                ))}
            </List>
        );
    } else {
        return (
            <NoSsr>
                <AutoSizer>
                    {({ height, width }) => (
                        <FixedSizeList
                            height={height}
                            width={width}
                            itemSize={itemSize}
                            itemCount={entries.length}
                            {...otherProps}
                        >
                            {renderRow}
                        </FixedSizeList>
                    )}
                </AutoSizer>
            </NoSsr>
        );
    }
}
const FilterList = React.memo(_FilterList, reactEqual);

// FIXME
// https://github.com/epoberezkin/fast-deep-equal/issues/49
function queryStringToBuildsFilterConfig(qstr) {
    const queryParsed = Object.assign({}, queryString.parse(qstr));
    const arrayTypeFields = [
        "item",
        "class",
        "mainSkill",
        "mainSkillSupport",
        "allSkill",
        "keystone",
        "weaponType",
        "offhandType",
        "orderBy"
    ];
    for (const filed of arrayTypeFields) {
        if (_hasIn(queryParsed, filed)) {
            queryParsed[filed] = _castArray(_get(queryParsed, filed, []));
        }
    }
    if (_hasIn(queryParsed, "mainSkillSupport")) {
        _set(
            queryParsed,
            "mainSkillSupport",
            _get(queryParsed, "mainSkillSupport", []).map((xs) => {
                if (_isString(xs)) {
                    return xs.split(",");
                } else {
                    return xs;
                }
            })
        );
    }
    return queryParsed;
}

function _PercentAvatars({ total, entries, config, pathname, isFetching }) {
    return (
        <FlipMove>
            {(entries || [])
                .filter((entry, index) => {
                    const { class: pclass, count } = entry;
                    return !(index > 18 && poeClasses.base.includes(pclass));
                })
                .sort((a, b) => {
                    const aClass = a.class;
                    const bClass = b.class;
                    const classes = _get(config, "class", []);
                    const aSelected = classes.includes(aClass);
                    const bSelected = classes.includes(bClass);
                    if (aSelected && bSelected) {
                        return b.count - a.count;
                    } else if (!aSelected && !bSelected) {
                        return 0;
                    } else if (aSelected && !bSelected) {
                        return -1;
                    } else if (!aSelected && bSelected) {
                        return 1;
                    } else {
                        return 0;
                    }
                })
                .map((entry, index) => {
                    const { class: pclass, count } = entry;
                    const avatarImg = PoeAvatars[pclass] || "";
                    const percentValue = Math.round((count / total) * 100);
                    const classes = _get(config, "class", []);
                    var nextClasses;
                    const selected = classes.includes(pclass);
                    if (selected) {
                        const s = new Set(classes);
                        s.delete(pclass);
                        nextClasses = [...s];
                    } else {
                        const s = new Set(classes);
                        s.add(pclass);
                        nextClasses = [...s];
                    }
                    const nextQuery = { ..._cloneDeep(config), class: nextClasses };
                    const to = isFetching
                        ? "#"
                        : queryString.stringifyUrl({
                              url: pathname,
                              query: nextQuery
                          });
                    return (
                        <IconButton
                            key={pclass}
                            disabled={isFetching}
                            style={{ padding: "14px 1px 14px 1px" }}
                            component={RouterLink}
                            replace
                            to={to}
                        >
                            <Tooltip arrow placement="top" title={pclass}>
                                <StyledBadge
                                    anchorOrigin={{
                                        vertical: "bottom",
                                        horizontal: "right"
                                    }}
                                    color={selected ? "secondary" : "primary"}
                                    badgeContent={
                                        (count === 0 && !selected) ||
                                        (isFetching && selected && count === 0) ||
                                        (isFetching && !selected)
                                            ? 0
                                            : `${percentValue === 0 ? ((count / total) * 100).toFixed(2) : percentValue}%`
                                    }
                                >
                                    <Avatar
                                        style={{
                                            width: 62,
                                            height: 62,
                                            filter: count === 0 && !selected ? "grayscale(100%)" : undefined
                                        }}
                                        src={avatarImg}
                                    />
                                </StyledBadge>
                            </Tooltip>
                        </IconButton>
                    );
                })}
        </FlipMove>
    );
}

const PercentAvatars = React.memo(_PercentAvatars, reactEqual);

const renderTooltip = (item, listItem) => {
    const node = treeData.getNode(item.skillId);
    if (node) {
        const statList = _get(node, "stats", [])
            .map((stat, i) => {
                return stat.split("\n").map((_stat, i2) => {
                    return <Typography key={`${i}-${i2}`}>{stat}</Typography>;
                });
            })
            .flat();
        const title = (
            <div
                style={{
                    display: "flex",
                    flexDirection: "column",
                    justifyContent: "center",
                    alignItems: "center"
                }}
            >
                <Typography>{node.name}</Typography>
                <div style={{ position: "relative", width: 133, height: 136 }}>
                    <div
                        style={{
                            position: "absolute",
                            width: 133,
                            height: 136,
                            backgroundImage: `url(https://web.poecdn.com/image/Art/2DArt/UIImages/InGame/PassiveSkillScreenKeystoneFrameCanAllocate.png?scale=1)`,
                            backgroundRepeat: "round",
                            zIndex: 2
                        }}
                    />
                    <img
                        style={{ position: "absolute", left: 28, top: 29 }}
                        src={`https://web.poecdn.com/image/${node.icon}?scale=1`}
                    />
                </div>
                {statList}
            </div>
        );
        return (
            <LazyTooltip placement="right" renderTitle={() => title}>
                {listItem}
            </LazyTooltip>
        );
    } else {
        return listItem;
    }
};

const _getConfig = (props) => {
    const config = queryStringToBuildsFilterConfig(props.location.search);
    if (props.defaultLeagueName !== props.leagueName) {
        config.leagueName = props.leagueName;
    }
    return config;
};

function decodeOp(str) {
    if (isInlucdeString(str)) {
        return { value: str, raw: str, isInclude: true };
    } else {
        return { value: str.substr(1), raw: str, isInclude: false };
    }
}

function encodeOp(str, isInclude = true) {
    if (isInlucdeString(str) && !isInclude) {
        return `!${str}`;
    } else if (!isInlucdeString(str) && isInclude) {
        return str.substr(1);
    } else {
        return str;
    }
}

function toggleOp(str) {
    const x = decodeOp(str);
    return encodeOp(str, !x.isInclude);
}

function isInlucdeString(str) {
    return str.charAt(0) !== "!";
}

const classAnalysisMeta = {
    name: "classCountEntries",
    configField: "class",
    configType: "array",
    getDisplay: (entry) => entry.class,
    getConfigValue: (entry) => entry.class
};

const searchAnalysisMetas = [
    {
        name: "uniqueCountEntries",
        configField: "item",
        configType: "array",
        getDisplay: (entry) => entry.itemName,
        getConfigValue: (entry, isInclude) => encodeOp(entry.itemName, isInclude)
    },
    {
        name: "mainSkillCountEntries",
        configField: "mainSkill",
        configType: "array",
        getDisplay: (entry) => entry.nameSpec,
        getConfigValue: (entry, isInclude) => encodeOp(entry.skillId, isInclude)
    },
    {
        name: "mainSkillSupportCountEntries",
        configField: "mainSkillSupport",
        configType: "mainSkillSupportArray",
        getDisplay: (entry) => entry.nameSpec,
        getConfigValue: (entry, isInclude) => encodeOp(entry.skillId, isInclude)
    },
    {
        name: "allSkillCountEntries",
        configField: "allSkill",
        configType: "array",
        getDisplay: (entry) => entry.nameSpec,
        getConfigValue: (entry, isInclude) => encodeOp(entry.skillId, isInclude)
    },
    {
        name: "allKeystoneCountEntries",
        configField: "keystone",
        configType: "array",
        getDisplay: (entry) => _get(treeData.getNode(entry.skillId), "name", ""),
        getConfigValue: (entry, isInclude) => encodeOp(entry.skillId, isInclude)
    },
    {
        name: "weaponTypeCountEntries",
        configType: "weapon/offhand",
        configField: ["weaponType", "offhandType"],
        getConfigValue: (entry, configField, isInclude) => {
            return encodeOp(entry[configField], isInclude);
        },
        getDisplay: (entry) => {
            const getNormalized = (str) => {
                return str.replace(/((^One Handed (.+)$)|(^Rune (Dagger)$))/, (match, p1, p2, p3, p4, p5) => {
                    return p3 || p5 || match;
                });
            };
            const weaponType = getNormalized(entry.weaponType);
            const offhandType = getNormalized(entry.offhandType);
            return `${weaponType} / ${offhandType}`;
        }
    }
];

const allAnalysisMeta = [classAnalysisMeta].concat(searchAnalysisMetas);

const allAnalysisMetaMapping = allAnalysisMeta.reduce((mapping, v) => {
    mapping[v.name] = v;
    return mapping;
}, {});

function BuildsPagePart2(props) {
    const { isFetching, analysis: analysisOri, total, location } = props;
    const uniqueKey = `${total},${(props.entries || []).map((x) => x.characterId).join(",")}`;
    const pageLimit = 50;

    const [analysis, setAnalysis] = React.useState(analysisOri);
    React.useEffect(() => {
        setAnalysis(analysisOri);
    }, [uniqueKey]);

    const [searchFilterIsFocus, setSearchFilterIsFocus] = React.useState(false);
    const [searchFilterText, setSearchFilterText] = React.useState("");
    const [searchFilterValue] = useDebounce(searchFilterText, 300);
    React.useEffect(() => {
        setSearchFilterText("");
    }, [props.leagueName, location.search]);

    const config = React.useMemo(() => _getConfig(props), [location.search]);

    const firstUpdate = React.useRef(true);
    React.useEffect(() => {
        if (firstUpdate.current) {
            firstUpdate.current = false;
        } else {
            props.putCharacters(props.leagueName, config);
        }
    }, [props.leagueName, location.search]);

    const getFuses = () => {
        return searchAnalysisMetas.map(({ name, getDisplay }) => {
            const options = {
                threshold: 0.4
            };
            const displays = _get(analysisOri, name, []).map(getDisplay);
            return new Fuse(displays, options);
        });
    };

    const [fuses, setFuses] = React.useState([]);

    React.useEffect(() => {
        setFuses([]);
    }, [uniqueKey]);
    React.useEffect(() => {
        if (searchFilterIsFocus) {
            setFuses(getFuses());
        }
    }, [searchFilterIsFocus]);

    React.useEffect(() => {
        if (searchFilterValue) {
            const searchResult = fuses.map((fuse) => fuse.search(searchFilterValue));
            var nextPartialAnalysis = {};
            for (const [index, { name }] of searchAnalysisMetas.entries()) {
                const nextEntries = searchResult[index].map(({ refIndex }) => _get(analysisOri, [name, refIndex]));
                nextPartialAnalysis[name] = nextEntries;
            }
            setAnalysis({ ...analysis, ...nextPartialAnalysis });
        } else {
            setAnalysis(analysisOri);
        }
    }, [searchFilterValue]);

    const [characterNameLikeText, setCharacterNameLikeText] = React.useState("");
    const [characterNameLikeValue] = useDebounce(characterNameLikeText, 500);

    const getPagination = (pagePosition = "top") => {
        const page = parseInt(_get(config, "offset", 0) / pageLimit + 1);
        if (page === 1 && pagePosition === "top") {
            return null;
        } else {
            const spacingStyle = pagePosition === "bottom" ? { padding: 25, marginTop: 5 } : {};
            return (
                <Pagination
                    disabled={isFetching}
                    page={page}
                    count={total % pageLimit === 0 ? parseInt(total / pageLimit) : parseInt(total / pageLimit) + 1}
                    color="secondary"
                    variant="outlined"
                    style={{ ...spacingStyle, userSelect: "none" }}
                    showFirstButton
                    showLastButton={pagePosition === "bottom"}
                    renderItem={(item) => {
                        if (_get(item, "page", null) !== null) {
                            const _offset = (item.page - 1) * pageLimit;
                            const offset = _offset > props.total ? props.total : _offset;
                            return (
                                <PaginationItem
                                    {...item}
                                    onClick={() => {
                                        if (page !== item.page) {
                                            window.scrollTo(0, 0);
                                        }
                                    }}
                                    component={RouterLink}
                                    to={{
                                        pathname: location.pathname,
                                        search: `?${queryString.stringify({
                                            ..._cloneDeep(config),
                                            offset: offset > 0 ? offset : undefined
                                        })}`
                                    }}
                                />
                            );
                        } else {
                            return <PaginationItem {...item} />;
                        }
                    }}
                />
            );
        }
    };
    const topPagination = React.useMemo(() => getPagination("top"), [isFetching, props.total, config.offset]);
    const bottomPagination = React.useMemo(() => getPagination("bottom"), [isFetching, props.total, config.offset]);

    return (
        <React.Fragment>
            <Helmet>
                <title>Builds - poetabby</title>
            </Helmet>
            <MyAppBar isFetching={isFetching} />
            <Container maxWidth="lg" style={{ paddingTop: 32 }}>
                <Grid container spacing={1}>
                    <Grid item xs={12}>
                        <Grid container>
                            <Grid item lg={3} md={3} xs={12}></Grid>
                            <Grid item md={7} xs={12}>
                                <Sticky stickyStyle={{ top: 48, zIndex: 2, marginLeft: 170 }}>
                                    <Paper elevation={4}>
                                        {allAnalysisMeta
                                            .map(({ name, configType, configField, getDisplay, getConfigValue }) => {
                                                const Tag = ({ label, nextQuery, toggleQuery, isInclude = true }) => {
                                                    return (
                                                        <EnhancedChip
                                                            label={label}
                                                            variant="outlined"
                                                            color="primary"
                                                            deleteIcon={
                                                                <React.Fragment>
                                                                    <Checkbox
                                                                        indeterminate
                                                                        checked={!isInclude}
                                                                        color="secondary"
                                                                        size="small"
                                                                        component={RouterLink}
                                                                        to={
                                                                            isFetching
                                                                                ? "#"
                                                                                : queryString.stringifyUrl({
                                                                                      url: location.pathname,
                                                                                      query: toggleQuery
                                                                                  })
                                                                        }
                                                                        replace
                                                                    />
                                                                    <RouterLink
                                                                        to={
                                                                            isFetching
                                                                                ? "#"
                                                                                : queryString.stringifyUrl({
                                                                                      url: location.pathname,
                                                                                      query: nextQuery
                                                                                  })
                                                                        }
                                                                        replace
                                                                    >
                                                                        <CancelIcon />
                                                                    </RouterLink>
                                                                </React.Fragment>
                                                            }
                                                            onDelete={() => {}}
                                                        />
                                                    );
                                                };
                                                if (configType === "array") {
                                                    return _get(config, configField, []).map((cvalue, cindex, ary) => {
                                                        const s = new Set(ary);
                                                        s.delete(encodeOp(cvalue, true));
                                                        s.delete(encodeOp(cvalue, false));
                                                        const cancelQuery = { ..._cloneDeep(config), [configField]: [...s] };
                                                        const toggleQuery = {
                                                            ..._cloneDeep(config),
                                                            [configField]: _set(_cloneDeep(ary), cindex, toggleOp(cvalue))
                                                        };
                                                        const getEntry = () => {
                                                            const entries = _get(analysisOri, name, []);
                                                            return entries.find((entry) => {
                                                                return getConfigValue(entry) === decodeOp(cvalue).value;
                                                            });
                                                        };
                                                        const labelPart1 =
                                                            configField[0].toUpperCase() + configField.substring(1);
                                                        const entry = getEntry();
                                                        var labelPart2;
                                                        if (Boolean(entry)) {
                                                            labelPart2 = getDisplay(entry);
                                                        } else {
                                                            const _dis = getDisplay({ skillId: decodeOp(cvalue).value });
                                                            if (_dis) {
                                                                labelPart2 = _dis;
                                                            } else {
                                                                labelPart2 = decodeOp(cvalue).value;
                                                            }
                                                        }
                                                        const label = `${labelPart1}: ${labelPart2}`;
                                                        return (
                                                            <Tag
                                                                key={label}
                                                                label={label}
                                                                nextQuery={cancelQuery}
                                                                toggleQuery={toggleQuery}
                                                                isInclude={decodeOp(cvalue).isInclude}
                                                            />
                                                        );
                                                    });
                                                } else if (configType === "weapon/offhand") {
                                                    const configFields = ["weaponType", "offhandType"];
                                                    const weaponTypes = _get(config, "weaponType", []);
                                                    const offhandTypes = _get(config, "offhandType", []);
                                                    if (weaponTypes.length === offhandTypes.length) {
                                                        const cvalues = _zipWith(
                                                            weaponTypes,
                                                            offhandTypes,
                                                            (weaponType, offhandType) => {
                                                                return { weaponType, offhandType };
                                                            }
                                                        );
                                                        return cvalues.map((cvalue, cindex) => {
                                                            const { weaponType, offhandType } = cvalue;

                                                            const cancelQuery = configFields.reduce(
                                                                (cancelQuery, configField) => {
                                                                    const q = _cloneDeep(cancelQuery);
                                                                    const s = new Set(_get(q, configField, []));
                                                                    const v = cvalue[configField];
                                                                    s.delete(encodeOp(v, true));
                                                                    s.delete(encodeOp(v, false));
                                                                    return {
                                                                        ...q,
                                                                        [configField]: [...s]
                                                                    };
                                                                },
                                                                _cloneDeep(config)
                                                            );

                                                            const toggleQuery = configFields.reduce(
                                                                (toggleQuery, configField) => {
                                                                    const q = _cloneDeep(toggleQuery);
                                                                    return {
                                                                        ...q,
                                                                        [configField]: _set(
                                                                            _get(q, configField, []),
                                                                            cindex,
                                                                            toggleOp(cvalue[configField])
                                                                        )
                                                                    };
                                                                },
                                                                _cloneDeep(config)
                                                            );
                                                            const label = getDisplay({
                                                                weaponType: decodeOp(weaponType).value,
                                                                offhandType: decodeOp(offhandType).value
                                                            });
                                                            return (
                                                                <Tag
                                                                    key={label}
                                                                    label={label}
                                                                    nextQuery={cancelQuery}
                                                                    toggleQuery={toggleQuery}
                                                                    isInclude={
                                                                        decodeOp(weaponType).isInclude &&
                                                                        decodeOp(offhandType).isInclude
                                                                    }
                                                                />
                                                            );
                                                        });
                                                    } else {
                                                        return null;
                                                    }
                                                } else {
                                                    return null;
                                                }
                                            })
                                            .flat()}
                                    </Paper>
                                </Sticky>
                            </Grid>
                            <Grid item md={2} xs={12}>
                                <div style={{ display: "flex", justifyContent: "flex-end", alignItems: "center", gap: 10 }}>
                                    <LeaguesMenu />
                                    <Sticky stickyStyle={{ top: 48, marginLeft: 5, zIndex: 2 }}>
                                        <Button
                                            disabled={isFetching || Object.keys(config).length === 0}
                                            variant="contained"
                                            color="primary"
                                            disableRipple
                                            component={RouterLink}
                                            to="/"
                                            replace={location.search === ""}
                                            onClick={() => {
                                                setSearchFilterText("");
                                            }}
                                        >
                                            Reset
                                        </Button>
                                    </Sticky>
                                </div>
                            </Grid>
                        </Grid>
                    </Grid>
                    <Grid item xs={12}>
                        <Paper
                            style={{
                                width: "100%",
                                maxWidth: "100%"
                            }}
                        >
                            {_hasIn(analysis, "classCountEntries") ? (
                                <PercentAvatars
                                    total={total}
                                    entries={_get(analysis, "classCountEntries", [])}
                                    config={config}
                                    isFetching={isFetching}
                                    pathname={location.pathname}
                                />
                            ) : isFetching ? (
                                <div style={{ display: "flex" }}>
                                    {[...Array(19).keys()].map((i) => (
                                        <div key={i} style={{ padding: "14px 1px 14px 1px" }}>
                                            <Skeleton variant="circle" width={62} height={62} />
                                        </div>
                                    ))}
                                </div>
                            ) : null}
                        </Paper>
                    </Grid>
                    <Grid item lg={3} md={3} xs={12}>
                        <Grid container direction="column" spacing={1}>
                            <Grid item>
                                <Sticky stickyStyle={{ top: 48, marginLeft: 5, zIndex: 2 }}>
                                    <Paper elevation={3}>
                                        <TextField
                                            id="filter-search"
                                            disabled={isFetching}
                                            value={searchFilterText}
                                            onFocus={() => {
                                                setSearchFilterIsFocus(true);
                                            }}
                                            onBlur={() => {
                                                setSearchFilterIsFocus(false);
                                            }}
                                            onChange={(e) => {
                                                setSearchFilterText(e.target.value);
                                            }}
                                            inputProps={{ spellCheck: "false" }}
                                            label="Search filter..."
                                            type="search"
                                            variant="outlined"
                                            size="small"
                                            fullWidth
                                            noValidate
                                            autoComplete="off"
                                        />
                                    </Paper>
                                </Sticky>
                            </Grid>
                            <Grid item>
                                <Paper
                                    style={{
                                        maxHeight: 424,
                                        width: "100%",
                                        maxWidth: "100%"
                                    }}
                                >
                                    <div style={{ height: 24, maxHeight: 24 }}>
                                        <Typography>Item</Typography>
                                    </div>
                                    <div
                                        style={{
                                            height:
                                                isFetching && _get(analysis, "uniqueCountEntries", []).length === 0
                                                    ? 400
                                                    : _get(analysis, "uniqueCountEntries", []).length * 40,
                                            maxHeight: 400
                                        }}
                                    >
                                        <FilterList
                                            total={total}
                                            isFetching={isFetching}
                                            entries={_get(analysis, "uniqueCountEntries", [])}
                                            config={config}
                                            configField="item"
                                            configType="array"
                                            pathname={location.pathname}
                                        />
                                    </div>
                                </Paper>
                            </Grid>
                            <Grid item>
                                <Paper
                                    style={{
                                        maxHeight: 424,
                                        width: "100%",
                                        maxWidth: "100%"
                                    }}
                                >
                                    <div style={{ height: 24, maxHeight: 24 }}>
                                        <Typography>Main Skill(5 link+)</Typography>
                                    </div>
                                    <div
                                        style={{
                                            height:
                                                isFetching && _get(analysis, "mainSkillCountEntries", []).length === 0
                                                    ? 400
                                                    : _get(analysis, "mainSkillCountEntries", []).length * 40,
                                            maxHeight: 400
                                        }}
                                    >
                                        <FilterList
                                            total={total}
                                            isFetching={isFetching}
                                            entries={_get(analysis, "mainSkillCountEntries", [])}
                                            config={config}
                                            configField="mainSkill"
                                            configType="array"
                                            pathname={location.pathname}
                                        />
                                    </div>
                                </Paper>
                            </Grid>
                            {_get(analysis, "mainSkillSupportCountEntries", []).map((mainSkillSupportCountEntry) => {
                                if (!mainSkillSupportCountEntry) {
                                    return null;
                                }
                                const { mainSkillId, supportCountEntries } = mainSkillSupportCountEntry;
                                return (
                                    <Grid key={mainSkillId} item>
                                        <Paper
                                            style={{
                                                maxHeight: 424,
                                                width: "100%",
                                                maxWidth: "100%"
                                            }}
                                        >
                                            <div style={{ height: 24, maxHeight: 24 }}>
                                                <Typography>{mainSkillId} Supports</Typography>
                                            </div>
                                            <div
                                                style={{
                                                    height: supportCountEntries.length * 40,
                                                    maxHeight: 400
                                                }}
                                            >
                                                <FilterList
                                                    total={_get(
                                                        _get(analysis, "mainSkillCountEntries", []).find(
                                                            (x) => x.skillId === mainSkillId
                                                        ),
                                                        "count",
                                                        0
                                                    )}
                                                    isFetching={isFetching}
                                                    entries={supportCountEntries.map((entry) => ({ ...entry, mainSkillId }))}
                                                    config={config}
                                                    getConfigValue={_get(allAnalysisMetaMapping, [
                                                        "mainSkillSupportCountEntries",
                                                        "getConfigValue"
                                                    ])}
                                                    configField="mainSkillSupport"
                                                    configType="mainSkillSupportArray"
                                                    pathname={location.pathname}
                                                />
                                            </div>
                                        </Paper>
                                    </Grid>
                                );
                            })}
                            <Grid item>
                                <Paper
                                    style={{
                                        maxHeight: 424,
                                        width: "100%",
                                        maxWidth: "100%"
                                    }}
                                >
                                    <div style={{ height: 24, maxHeight: 24 }}>
                                        <Typography>Keystones&nbsp;/&nbsp;Ascendency</Typography>
                                    </div>
                                    <div
                                        style={{
                                            height: _get(analysis, "allKeystoneCountEntries", []).length * 40,
                                            maxHeight: 400
                                        }}
                                    >
                                        <FilterList
                                            total={total}
                                            isFetching={isFetching}
                                            entries={_get(analysis, "allKeystoneCountEntries", [])}
                                            config={config}
                                            configField="keystone"
                                            configType="array"
                                            pathname={location.pathname}
                                            renderTooltip={renderTooltip}
                                        />
                                    </div>
                                </Paper>
                            </Grid>
                            <Grid item>
                                <Paper
                                    style={{
                                        maxHeight: 400,
                                        height: _get(analysis, "allSkillCountEntries", []).length * 400,
                                        width: "100%",
                                        maxWidth: "100%"
                                    }}
                                >
                                    <FilterList
                                        isFetching={isFetching}
                                        total={total}
                                        entries={_get(analysis, "allSkillCountEntries", [])}
                                        config={config}
                                        configField="allSkill"
                                        configType="array"
                                        pathname={location.pathname}
                                    />
                                </Paper>
                            </Grid>
                            <Grid item>
                                <Paper
                                    style={{
                                        maxHeight: 400,
                                        height: _get(analysis, "weaponTypeCountEntries", []).length * 40,
                                        width: "100%",
                                        maxWidth: "100%"
                                    }}
                                >
                                    <FilterList
                                        isFetching={isFetching}
                                        total={total}
                                        entries={_get(analysis, "weaponTypeCountEntries", [])}
                                        config={config}
                                        configType="weapon/offhand"
                                        getConfigValue={allAnalysisMetaMapping["weaponTypeCountEntries"].getConfigValue}
                                        pathname={location.pathname}
                                    />
                                </Paper>
                            </Grid>
                        </Grid>
                    </Grid>
                    <Grid item lg={9} md={9} xs={12}>
                        <Grid container direction="column" spacing={1}>
                            <Grid item>
                                <Paper style={{ display: "flex", alignItems: "center", gap: 5 }}>
                                    <TextField
                                        label="Name (not implemented)"
                                        type="search"
                                        variant="outlined"
                                        size="small"
                                        noValidate
                                        autoComplete="off"
                                        inputProps={{ spellCheck: "false" }}
                                    />
                                </Paper>
                            </Grid>
                            <Grid item>
                                <Paper style={{ display: "flex", alignItems: "center", gap: 5 }}>
                                    <Typography>
                                        Found <strong>{total}</strong> characters.
                                    </Typography>
                                    {topPagination}
                                </Paper>
                            </Grid>
                            <Grid item>
                                <Paper>
                                    <CharacterTable
                                        isFetching={isFetching}
                                        data={props.entries}
                                        putCharacterData={props.putCharacterData}
                                        backBuildsPageUrl={queryString.stringifyUrl({
                                            url: location.pathname,
                                            query: config
                                        })}
                                        fromLocation={location}
                                        charLinkLocationState={location.state}
                                        config={config}
                                        orderBy={_get(config, "orderBy")}
                                    />
                                    {props.total === 0 && isFetching ? (
                                        <div>
                                            {[...Array(pageLimit).keys()].map((i) => (
                                                <Skeleton key={i} width="100%" height={60} />
                                            ))}
                                        </div>
                                    ) : null}
                                    {bottomPagination}
                                </Paper>
                            </Grid>
                        </Grid>
                        <Grid item xs={12}>
                            <div style={{ width: "100%", height: "20vh" }}></div>
                        </Grid>
                    </Grid>
                </Grid>
            </Container>
        </React.Fragment>
    );
}

class BuildsPage extends React.Component {
    constructor(props) {
        super(props);
        if (props.readyFirstRenderOnServer || !isNode()) {
            const config = _getConfig(props);
            wait(async () => {
                if (!isNode() && _get(props, "total", 0) > 0) {
                    this.props.cancelDelayClear();
                } else {
                    await props.putCharacters(props.leagueName, config);
                }
            }, "BuildPage init Task");
        }
    }

    componentDidMount() {
        this.props.cancelDelayClear();
    }

    componentWillUnmount() {
        this.props.delayClear();
    }

    render() {
        if (this.props.readyFirstRenderOnServer) {
            return null;
        } else {
            return <BuildsPagePart2 {...this.props} />;
        }
    }
}

const connectedComponent = connect(
    (state) => {
        return { ...state.league, ...state.serverEnv };
    },
    { ...LeagueStore.actionCreators, putCharacterData: CharacterStore.actionCreators.putCharacterData }
)(BuildsPage);

export default withRouter(connectedComponent);
