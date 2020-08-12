import * as React from "react";
import { Helmet } from "react-helmet";
import queryString from "query-string";
import { goBack as routerGoBack, push as routerPush } from "connected-react-router";

import { makeStyles, withStyles } from "@material-ui/core/styles";

import * as CharacterStore from "@Store/characterStore";
import { connect } from "react-redux";
import { wait } from "domain-wait";

import { withRouter } from "react-router";
import { Link as RouterLink } from "react-router-dom";
import NotFoundPage from "@Pages/NotFoundPage";
import Link from "@material-ui/core/Link";
import HomeIcon from "@material-ui/icons/Home";

import Container from "@material-ui/core/Container";

import Paper from "@material-ui/core/Paper";
import Typography from "@material-ui/core/Typography";

import Table from "@material-ui/core/Table";
import TableBody from "@material-ui/core/TableBody";
import TableCell from "@material-ui/core/TableCell";
import TableContainer from "@material-ui/core/TableContainer";
import TableHead from "@material-ui/core/TableHead";
import TableRow from "@material-ui/core/TableRow";

import Grid from "@material-ui/core/Grid";

import Breadcrumbs from "@material-ui/core/Breadcrumbs";

import Tooltip from "@material-ui/core/Tooltip";

import Divider from "@material-ui/core/Divider";

import Avatar from "@material-ui/core/Avatar";
import * as PoeAvatars from "@Images/poeAvatars";
import { getDividerByFrameType } from "@Images/poeDividers";

import _get from "lodash/get";
import _hasIn from "lodash/hasIn";

import SimplePassiveTree from "@Components/character/SimplePassiveTree";

import Skeleton from "@material-ui/lab/Skeleton";

import moment from "moment";
import { isNode } from "@Utils";

import Button from "@material-ui/core/Button";
import * as clipboard from "clipboard-polyfill";
import FileCopyIcon from "@material-ui/icons/FileCopy";
import { toast } from "react-toastify";

import Timeline from "@material-ui/lab/Timeline";
import TimelineItem from "@material-ui/lab/TimelineItem";
import TimelineSeparator from "@material-ui/lab/TimelineSeparator";
import TimelineConnector from "@material-ui/lab/TimelineConnector";
import TimelineContent from "@material-ui/lab/TimelineContent";
import TimelineOppositeContent from "@material-ui/lab/TimelineOppositeContent";
import TimelineDot from "@material-ui/lab/TimelineDot";

import MyAppBar from "@Components/shared/MyAppBar";

import LazyLoad from "react-lazyload";

const useMyTimelineItemStyles = makeStyles((theme) => ({
    missingOppositeContent: {
        "&:before": {
            padding: 0,
            flex: 0
        }
    }
}));

function MyTimelineItem(props) {
    const classes = useMyTimelineItemStyles();
    return <TimelineItem classes={classes} {...props} />;
}

const useMyTooltipStyles = makeStyles((theme) => ({
    arrow: {
        color: "#151515"
    },
    tooltip: {
        padding: 3,
        backgroundColor: "rgba(15,15,15,0.94)",
        border: "2px solid gray",
        maxWidth: "40vw",
        textAlign: "center"
    }
}));

function ItemTooltip(props) {
    const classes = useMyTooltipStyles();
    return <Tooltip arrow classes={classes} {...props} />;
}

function getFrameTypeColor(frameType) {
    const borderColors = {
        0: "#E9EBEE",
        1: "#8888ff",
        2: "#CCCC00",
        3: "#AF6025"
    };
    return borderColors[frameType] || borderColors[0];
}

function getItemHeaderBackgroundColor(frameType) {
    const borderColors = {
        0: "#E9EBEE",
        1: "#8888ff",
        2: "#CCCC00",
        3: "#AF6025"
    };
    return borderColors[frameType] || borderColors[0];
}

class UpdateDateTimeDisplay extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            _hasMounted: false
        };
    }

    componentDidMount() {
        if (!this.state._hasMounted) {
            this._timer = setTimeout(() => {
                this.setState({ _hasMounted: true });
            }, 0);
        }
    }

    componentWillUnmount() {
        if (this._timer) {
            clearTimeout(this._timer);
            this._timer = null;
        }
    }

    render() {
        if (isNode() || !this.state._hasMounted) {
            const placeholder = this.props.placeholder || "";
            return placeholder;
        } else {
            const dateTime = this.props.dateTime;
            const nowUtc = moment().utc();
            const updatedAtUtc = moment(`${dateTime}+0000`);
            const elapsedTime = updatedAtUtc.from(nowUtc).toString();
            return (
                <Tooltip title={updatedAtUtc.format("lll")}>
                    <span>{elapsedTime}</span>
                </Tooltip>
            );
        }
    }
}

const ItemDivider = (props) => {
    const { frameType: _frameType, fullWidth: _fullWidth, component: _component, style: _style, ...otherProps } = props;
    const fullWidth = _fullWidth || true;
    const Component = _component || "div";
    const frameType = _frameType || 0;
    const dividerImage = getDividerByFrameType(frameType);
    const __style = _style || {};
    const style = {
        backgroundImage: `url(${dividerImage})`,
        width: fullWidth ? "100%" : undefined,
        height: "8px",
        backgroundRepeat: "no-repeat",
        backgroundPosition: "center",
        ...__style
    };
    return <Component style={style} {...otherProps} />;
};

const ItemDetail = ({ data }) => {
    const {
        id,
        frameType,
        name,
        typeLine,
        ilvl,
        properties: _properties,
        explicitMods: _explicitMods,
        implicitMods: _implicitMods,
        craftedMods: _craftedMods,
        enchantMods: _enchantMods,
        utilityMods: _utilityMods
    } = data;
    const properties = _properties || [];
    const getPropValueColor = (_i) => {
        const i = _i || 0;
        const propValueColorMapping = { 0: "#FFFFFF", 1: "#6565CC", 4: "#960000", 5: "#366492", 6: "#ffd700", 7: "#d02090" };
        return propValueColorMapping[i];
    };
    const explicitMods = _explicitMods || [];
    const implicitMods = _implicitMods || [];
    const craftedMods = _craftedMods || [];
    const enchantMods = _enchantMods || [];
    const utilityMods = _utilityMods || [];
    return (
        <div style={{ display: "flex", alignItems: "center", flexDirection: "column" }}>
            <div
                style={{
                    display: "flex",
                    alignItems: "center",
                    flexDirection: "column",
                    width: "100%",
                    height: "100%"
                }}
            >
                {name ? (
                    <Typography key={name} style={{ color: getFrameTypeColor(frameType), fontSize: "18px" }}>
                        {name}
                    </Typography>
                ) : null}
                {typeLine ? (
                    <Typography key={typeLine} style={{ color: getFrameTypeColor(frameType), fontSize: "18px" }}>
                        {typeLine}
                    </Typography>
                ) : null}
            </div>
            {properties.length > 0 || (ilvl !== null && ilvl !== undefined && ilvl !== 0) ? (
                <ItemDivider frameType={frameType} />
            ) : null}
            {properties.map((p) => {
                const values = p.values || [];
                const name = p.name;
                const splitedName = name.split(/(?!%%)(%\d+)/g).filter((t) => t);
                if (splitedName.length == 1) {
                    const vDoms = values.map((vpair, index) => {
                        const v = vpair[0];
                        const vColor = getPropValueColor(vpair[1]);
                        return (
                            <React.Fragment key={`vdoms-${index}`}>
                                <span style={{ color: vColor }}>{v}</span>
                                {index < values.length - 1 ? <span style={{ color: "#808080" }}>,&nbsp;</span> : null}
                            </React.Fragment>
                        );
                    });
                    return (
                        <Typography key={name} variant="body1">
                            <span style={{ color: "#808080" }}>
                                {name}
                                {values.length > 0 ? ":" : null}
                                &nbsp;
                            </span>
                            {values.length > 0 ? vDoms : null}
                        </Typography>
                    );
                } else {
                    return (
                        <Typography key={name} variant="body1">
                            {splitedName.map((partName, index) => {
                                const _sindex = _get(/%(\d+)/.exec(partName), [1], null);
                                const sindex = _sindex !== null ? parseInt(_sindex) : null;
                                const sv = sindex !== null ? _get(values, [sindex, 0], null) : null;
                                const svColor = sv !== null ? _get(values, [sindex, 1], null) : null;
                                return sv !== null ? (
                                    <span key={`${partName}-${index}`} style={{ color: getPropValueColor(svColor) }}>
                                        {sv}
                                    </span>
                                ) : (
                                    <span key={`${partName}-${index}`} style={{ color: "#808080" }}>
                                        {partName}
                                    </span>
                                );
                            })}
                        </Typography>
                    );
                }
            })}
            {ilvl !== null && ilvl !== undefined && ilvl !== 0 ? (
                <Typography key={`ilvl-${ilvl}`} variant="body1">
                    <span style={{ color: "#808080" }}>Item Level:</span>&nbsp;{ilvl}
                </Typography>
            ) : null}
            {enchantMods.length > 0 ? <ItemDivider frameType={frameType} /> : null}
            {enchantMods.map((mod) => {
                return (
                    <Typography key={mod} variant="body1" style={{ color: "#7DA5FF" }}>
                        {mod}
                    </Typography>
                );
            })}
            {implicitMods.length > 0 ? <ItemDivider frameType={frameType} /> : null}
            {implicitMods.map((mod) => {
                return (
                    <Typography key={mod} variant="body1" style={{ color: "#6565CC" }}>
                        {mod}
                    </Typography>
                );
            })}
            {explicitMods.length > 0 ? <ItemDivider frameType={frameType} /> : null}
            {explicitMods.concat(utilityMods).map((mod) => {
                return (
                    <Typography key={mod} variant="body1" style={{ color: "#6565CC" }}>
                        {mod}
                    </Typography>
                );
            })}
            {craftedMods.map((mod) => {
                return (
                    <Typography key={mod} variant="body1" style={{ color: "#7DA5FF" }}>
                        {mod}
                    </Typography>
                );
            })}
        </div>
    );
};

function getPlayerStatValue(pob, statName, notFoundValue = undefined) {
    return _get(
        _get(pob, "build.playerStat", []).find((pstat) => pstat.stat == statName),
        "value",
        notFoundValue
    );
}

const Inventory = ({ data, component = "div" }) => {
    const Component = component || "div";
    const ps = {
        BodyArmour: {
            left: "252.059px",
            top: "206.138px"
        },
        Ring: {
            left: "182.688px",
            top: "253.602px"
        },
        Ring2: {
            left: "368.894px",
            top: "253.602px"
        },
        Gloves: {
            left: "135.223px",
            top: "312.629px"
        },
        Offhand: {
            left: "438.266px",
            top: "111.209px"
        },
        Boots: {
            left: "368.894px",
            top: "312.629px"
        },
        Belt: {
            left: "252.059px",
            top: "360.093px"
        },
        Weapon: {
            left: "65.8519px",
            top: "111.209px"
        },
        Amulet: {
            left: "368.894px",
            top: "194.576px"
        },
        Helm: {
            left: "252.059px",
            top: "99.6471px"
        }
    };
    const borderColors = {
        0: "#E9EBEE",
        1: "#8888ff",
        2: "#CCCC00",
        3: "#AF6025"
    };
    const flaskPs = [
        { left: "186.339px", top: "418.511px" },
        { left: "233.803px", top: "418.511px" },
        { left: "281.268px", top: "418.511px" },
        { left: "328.732px", top: "418.511px" },
        { left: "376.197px", top: "418.511px" }
    ];
    const flasks = data.filter((item) => item.inventoryId == "Flask");
    const flaskDoms = flasks.map((item) => {
        const p = flaskPs[item.x];
        if (p) {
            const borderColor = borderColors[item.frameType] || "black";
            const border = `2px solid ${borderColor}`;
            const style = { position: "absolute", left: p.left, top: p.top, border };
            return (
                <ItemTooltip key={item.id} placement="left" title={<ItemDetail data={item} />}>
                    <img key={item.id} style={style} src={item.icon} />
                </ItemTooltip>
            );
            return;
        } else {
            return null;
        }
    });
    const passiveJewels = data.filter((item) => item.inventoryId == "PassiveJewels");
    const equipKeys = Object.keys(ps);
    const equips = data.filter((item) => equipKeys.some((k) => k == item.inventoryId));
    const equipDoms = equips.map((item) => {
        const p = ps[item.inventoryId];
        const borderColor = borderColors[item.frameType] || "black";
        const border = `3px solid ${borderColor}`;
        var style = {
            position: "absolute",
            left: p.left,
            top: p.top,
            border,
            display: "flex",
            justifyContent: "center",
            alignItem: "center"
        };
        if (item.inventoryId == "Weapon" || item.inventoryId == "Offhand") {
            style = { ...style, width: 94, height: 181 };
        }
        return (
            <ItemTooltip key={item.id} placement="left" title={<ItemDetail data={item} />}>
                <div style={style}>
                    <img src={item.icon} style={{ objectFit: "scale-down" }} />
                </div>
            </ItemTooltip>
        );
    });
    return (
        <Component
            style={{
                position: "relative",
                width: 600,
                height: 531,
                backgroundImage: "url('//www.pathofexile.com/image/inventory/MainInventoryNoBags.png?1593393374306')"
            }}
        >
            {equipDoms}
            {flaskDoms}
        </Component>
    );
};

const DefaultTableCellSkeleton = (props) => {
    return <Skeleton variant="rect" width={50} height={20} {...props} />;
};

import Chip from "@material-ui/core/Chip";

class CharacterPage extends React.Component {
    constructor(props) {
        super(props);
        if (props.readyFirstRenderOnServer || !isNode()) {
            wait(async () => {
                const { accountName, characterName } = props.match.params;
                await props.putCharacter(accountName, characterName);
            }, "CharacterPage init Task");
        }
    }

    componentWillUnmount() {
        this.props.clear();
    }

    render() {
        if (this.props.readyFirstRenderOnServer) {
            return null;
        }
        if (this.props.lastError) {
            return <NotFoundPage />;
        }
        const characterName = this.props.character ? this.props.character.characterName : this.props.match.params.characterName;
        const accountName = this.props.character ? this.props.character.accountName : this.props.match.params.accountName;
        const { character: pchar, location } = this.props;
        return (
            <React.Fragment>
                <Helmet>
                    <title>{`${characterName} - poetabby`}</title>
                </Helmet>
                <MyAppBar />
                <Container maxWidth="lg" style={{ paddingTop: 32 }}>
                    <div style={{ display: "inline-flex", flexWrap: "wrap", gap: "12px" }}>
                        <Paper>
                            <Button
                                color="primary"
                                variant="contained"
                                href={_get(location, "state.backBuildsPageUrl") || "/"}
                                onClick={(e) => {
                                    e.preventDefault();
                                    const backBuildsPageUrl = _get(location, "state.backBuildsPageUrl");
                                    const fromLocation = _get(location, "state.from", { pathname: "/" });
                                    const prevUrl = queryString.stringifyUrl({
                                        url: fromLocation.pathname,
                                        query: queryString.parse(fromLocation.search)
                                    });
                                    if (prevUrl === backBuildsPageUrl && this.props.leagueEntryCount > 0) {
                                        this.props.history.goBack();
                                    } else {
                                        this.props.history.push(backBuildsPageUrl || "/");
                                    }
                                }}
                            >
                                Back To Search
                            </Button>
                        </Paper>
                        <Paper>
                            <Breadcrumbs style={{ paddingLeft: 8, paddingTop: 8 }}>
                                <Link component={RouterLink} variant="subtitle1" to="/">
                                    Builds
                                </Link>
                                <Link
                                    variant="subtitle1"
                                    href={`//www.pathofexile.com/account/view-profile/${accountName}`}
                                    target="_blank"
                                >
                                    <strong>
                                        <span style={{ color: "#A38D6D", userSelect: "none" }}>[</span>
                                    </strong>
                                    {accountName}
                                    <strong>
                                        <span style={{ color: "#A38D6D", userSelect: "none" }}>]</span>
                                    </strong>
                                </Link>
                                <div>
                                    <Typography
                                        variant="h5"
                                        style={{ color: "#A38D6D", userSelect: "none", display: "inline-block" }}
                                    >
                                        [
                                    </Typography>
                                    <Link
                                        variant="h5"
                                        href={`//www.pathofexile.com/account/view-profile/${accountName}/characters?characterName=${characterName}`}
                                        target="_blank"
                                        style={{ marginLeft: 3, marginRight: 3 }}
                                    >
                                        <strong>{characterName}</strong>
                                    </Link>
                                    <strong>
                                        <Typography
                                            variant="h5"
                                            style={{ color: "#A38D6D", userSelect: "none", display: "inline-block" }}
                                        >
                                            ]
                                        </Typography>
                                    </strong>
                                </div>
                            </Breadcrumbs>
                            <Grid container spacing={2} justify="flex-start" alignItems="flex-start" spacing={3}>
                                <Grid item md={3} xs={12}>
                                    <Table style={{ width: "auto", tableLayout: "auto" }}>
                                        <TableBody>
                                            <TableRow>
                                                <TableCell>Class</TableCell>
                                                <TableCell align="right">
                                                    {pchar ? (
                                                        <div
                                                            style={{
                                                                display: "flex",
                                                                justifyContent: "center",
                                                                alignItems: "center",
                                                                flexDirection: "column"
                                                            }}
                                                        >
                                                            <Avatar
                                                                style={{ width: 62, height: 62 }}
                                                                src={PoeAvatars[pchar.class]}
                                                            />
                                                            {pchar.class}
                                                        </div>
                                                    ) : (
                                                        <div
                                                            style={{
                                                                display: "flex",
                                                                justifyContent: "center",
                                                                alignItems: "center",
                                                                flexDirection: "column"
                                                            }}
                                                        >
                                                            <Skeleton variant="circle" width={62} height={62} />
                                                            <Skeleton variant="rect" width={87} height={16} />
                                                        </div>
                                                    )}
                                                </TableCell>
                                            </TableRow>
                                            <TableRow>
                                                <TableCell>Level</TableCell>
                                                <TableCell align="right">
                                                    {pchar ? pchar.level : <Skeleton variant="rect" width={87} height={16} />}
                                                </TableCell>
                                            </TableRow>
                                            <TableRow>
                                                <TableCell>League</TableCell>
                                                <TableCell align="right">
                                                    {pchar ? (
                                                        pchar.leagueName
                                                    ) : (
                                                        <Skeleton variant="rect" width={87} height={16} />
                                                    )}
                                                </TableCell>
                                            </TableRow>
                                        </TableBody>
                                    </Table>
                                </Grid>
                                <Grid item md={4} xs={12}>
                                    <Table style={{ width: "auto", tableLayout: "auto" }}>
                                        <TableBody>
                                            <TableRow>
                                                <TableCell>Update Date</TableCell>
                                                <TableCell align="right">
                                                    {pchar ? (
                                                        <UpdateDateTimeDisplay
                                                            dateTime={pchar.updatedAt}
                                                            placeholder={<DefaultTableCellSkeleton />}
                                                        />
                                                    ) : (
                                                        <DefaultTableCellSkeleton />
                                                    )}
                                                </TableCell>
                                            </TableRow>
                                            <TableRow>
                                                <TableCell>
                                                    {_hasIn(pchar, "pob.tree.spec.url") ? (
                                                        <Link href={`${_get(pchar, "pob.tree.spec.url").trim()}`} target="_blank">
                                                            Passive Tree
                                                        </Link>
                                                    ) : (
                                                        <span>Passive Tree</span>
                                                    )}
                                                </TableCell>
                                            </TableRow>
                                            <TableRow>
                                                <TableCell>
                                                    <Button
                                                        disabled={!_hasIn(pchar, "pobCode")}
                                                        variant="contained"
                                                        color="primary"
                                                        startIcon={<FileCopyIcon />}
                                                        onClick={(e) => {
                                                            const pobCode = _get(pchar, "pobCode");
                                                            if (pobCode) {
                                                                clipboard.writeText(pobCode).then(
                                                                    () => {
                                                                        toast.success("Copied to clipboard!", {
                                                                            position: "top-center",
                                                                            autoClose: 2000
                                                                        });
                                                                    },
                                                                    () => {
                                                                        toast.error("Failed copy to clipboard...", {
                                                                            position: "top-center",
                                                                            autoClose: 2000
                                                                        });
                                                                    }
                                                                );
                                                            }
                                                        }}
                                                        size="small"
                                                    >
                                                        Pob Code
                                                    </Button>
                                                </TableCell>
                                            </TableRow>
                                        </TableBody>
                                    </Table>
                                </Grid>
                                <Grid item md={5} xs={12} />
                                <Grid item md={3} xs={12}>
                                    <Table style={{ width: "auto", tableLayout: "auto" }}>
                                        <TableBody>
                                            <TableRow>
                                                <TableCell>Life</TableCell>
                                                <TableCell align="right">
                                                    {_hasIn(pchar, "lifeUnreserved") ? (
                                                        _get(pchar, "lifeUnreserved")
                                                    ) : (
                                                        <DefaultTableCellSkeleton />
                                                    )}
                                                </TableCell>
                                            </TableRow>
                                            <TableRow>
                                                <TableCell>Energy Shield</TableCell>
                                                <TableCell align="right">
                                                    {_hasIn(pchar, "energyShield") ? (
                                                        _get(pchar, "energyShield")
                                                    ) : (
                                                        <DefaultTableCellSkeleton />
                                                    )}
                                                </TableCell>
                                            </TableRow>
                                            <TableRow>
                                                <TableCell>Mana</TableCell>
                                                <TableCell align="right">
                                                    {_hasIn(pchar, "pob") ? (
                                                        getPlayerStatValue(pchar.pob, "Mana", "None")
                                                    ) : (
                                                        <DefaultTableCellSkeleton />
                                                    )}
                                                </TableCell>
                                            </TableRow>
                                            <TableRow>
                                                <TableCell>Armour</TableCell>
                                                <TableCell align="right">
                                                    {_hasIn(pchar, "pob") ? (
                                                        getPlayerStatValue(pchar.pob, "Armour", "None")
                                                    ) : (
                                                        <DefaultTableCellSkeleton />
                                                    )}
                                                </TableCell>
                                            </TableRow>
                                            <TableRow>
                                                <TableCell>Evasion Rating</TableCell>
                                                <TableCell align="right">
                                                    {_hasIn(pchar, "pob") ? (
                                                        getPlayerStatValue(pchar.pob, "Evasion", "None")
                                                    ) : (
                                                        <DefaultTableCellSkeleton />
                                                    )}
                                                </TableCell>
                                            </TableRow>
                                        </TableBody>
                                    </Table>
                                </Grid>
                                <Grid item md={3} xs={12}>
                                    <Table style={{ width: "auto", tableLayout: "auto" }}>
                                        <TableBody>
                                            <TableRow>
                                                <TableCell>Strength</TableCell>
                                                <TableCell align="right">
                                                    {_hasIn(pchar, "pob") ? (
                                                        getPlayerStatValue(pchar.pob, "Str", "None")
                                                    ) : (
                                                        <DefaultTableCellSkeleton />
                                                    )}
                                                </TableCell>
                                            </TableRow>
                                            <TableRow>
                                                <TableCell>Dexterity</TableCell>
                                                <TableCell align="right">
                                                    {_hasIn(pchar, "pob") ? (
                                                        getPlayerStatValue(pchar.pob, "Dex", "None")
                                                    ) : (
                                                        <DefaultTableCellSkeleton />
                                                    )}
                                                </TableCell>
                                            </TableRow>
                                            <TableRow>
                                                <TableCell>Intelligence</TableCell>
                                                <TableCell align="right">
                                                    {_hasIn(pchar, "pob") ? (
                                                        getPlayerStatValue(pchar.pob, "Int", "None")
                                                    ) : (
                                                        <DefaultTableCellSkeleton />
                                                    )}
                                                </TableCell>
                                            </TableRow>
                                        </TableBody>
                                    </Table>
                                </Grid>
                                <Grid item md={3} xs={12}>
                                    <Table style={{ width: "auto", tableLayout: "auto" }}>
                                        <TableBody>
                                            <TableRow>
                                                <TableCell>Block Chance</TableCell>
                                                <TableCell align="right">
                                                    {_hasIn(pchar, "pob") ? (
                                                        `${getPlayerStatValue(pchar.pob, "BlockChance", "None")}%`
                                                    ) : (
                                                        <DefaultTableCellSkeleton />
                                                    )}
                                                </TableCell>
                                            </TableRow>
                                            <TableRow>
                                                <TableCell>Spell Block Chance</TableCell>
                                                <TableCell align="right">
                                                    {_hasIn(pchar, "pob") ? (
                                                        `${getPlayerStatValue(pchar.pob, "SpellBlockChance", "None")}%`
                                                    ) : (
                                                        <DefaultTableCellSkeleton />
                                                    )}
                                                </TableCell>
                                            </TableRow>
                                            <TableRow>
                                                <TableCell>Doge Change</TableCell>
                                                <TableCell align="right">
                                                    {_hasIn(pchar, "pob") ? (
                                                        `${getPlayerStatValue(pchar.pob, "AttackDodgeChance", "None")}%`
                                                    ) : (
                                                        <DefaultTableCellSkeleton />
                                                    )}
                                                </TableCell>
                                            </TableRow>
                                            <TableRow>
                                                <TableCell>Spell Doge Change</TableCell>
                                                <TableCell align="right">
                                                    {_hasIn(pchar, "pob") ? (
                                                        `${getPlayerStatValue(pchar.pob, "SpellDodgeChance", "None")}%`
                                                    ) : (
                                                        <DefaultTableCellSkeleton />
                                                    )}
                                                </TableCell>
                                            </TableRow>
                                        </TableBody>
                                    </Table>
                                </Grid>
                                <Grid item md={3} xs={12}>
                                    <Table style={{ width: "auto", tableLayout: "auto" }}>
                                        <TableBody>
                                            <TableRow>
                                                <TableCell>Endurance Charges(.Max)</TableCell>
                                                <TableCell align="right">
                                                    {_hasIn(pchar, "pob") ? (
                                                        getPlayerStatValue(pchar.pob, "EnduranceChargesMax", "None")
                                                    ) : (
                                                        <Skeleton variant="rect" width="8" height="19" />
                                                    )}
                                                </TableCell>
                                            </TableRow>
                                            <TableRow>
                                                <TableCell>Frenzy Charges(.Max)</TableCell>
                                                <TableCell align="right">
                                                    {_hasIn(pchar, "pob") ? (
                                                        getPlayerStatValue(pchar.pob, "FrenzyChargesMax", "None")
                                                    ) : (
                                                        <Skeleton variant="rect" width="8" height="19" />
                                                    )}
                                                </TableCell>
                                            </TableRow>
                                            <TableRow>
                                                <TableCell>Power Charges(.Max)</TableCell>
                                                <TableCell align="right">
                                                    {_hasIn(pchar, "pob") ? (
                                                        getPlayerStatValue(pchar.pob, "PowerChargesMax", "None")
                                                    ) : (
                                                        <Skeleton variant="rect" width="8" height="19" />
                                                    )}
                                                </TableCell>
                                            </TableRow>
                                        </TableBody>
                                    </Table>
                                </Grid>
                            </Grid>
                        </Paper>
                        <Paper style={{ width: "100%" }}>
                            <div style={{ display: "flex", justifyContent: "center" }}>
                                {_hasIn(pchar, "items") ? (
                                    <Inventory data={pchar.items} />
                                ) : (
                                    <Skeleton variant="rect" width={600} height={530} />
                                )}
                            </div>
                        </Paper>
                        {_hasIn(pchar, "pob.tree.spec.nodes") || _hasIn(pchar, "treeNodes") ? (
                            <div style={{ width: "100%", height: "100%" }}>
                                <LazyLoad
                                    height="90vh"
                                    offset={0}
                                    placeholder={<Skeleton variant="rect" height="90vh" width="100%" />}
                                >
                                    <SimplePassiveTree
                                        data={_get(pchar, "treeNodes") || _get(pchar, "pob.tree.spec.nodes")}
                                        loading={<Skeleton variant="rect" height="90vh" width="100%" />}
                                    />
                                </LazyLoad>
                            </div>
                        ) : (
                            <Skeleton variant="rect" height="90vh" width="100%" />
                        )}
                        <Grid container spacing={2}>
                            {_hasIn(pchar, "socketedItemGroups")
                                ? _get(pchar, "socketedItemGroups", []).map((sg, index) => {
                                      const { inventoryId, item, groups } = sg;
                                      return (
                                          <Grid key={`${item.id}`} item xs={12} md={4}>
                                              <Paper elevation={3}>
                                                  <ItemTooltip placement="left" title={<ItemDetail data={item} />}>
                                                      <div
                                                          style={{
                                                              paddingTop: 5,
                                                              paddingLeft: 5,
                                                              display: "flex",
                                                              alignItems: "center",
                                                              backgroundColor: "rgba(1,2,3, 0.2)"
                                                          }}
                                                      >
                                                          <img
                                                              src={item.icon}
                                                              style={{
                                                                  maxWidth: "50px",
                                                                  maxHeight: "71px",
                                                                  objectFit: "scale-down"
                                                              }}
                                                          />
                                                          <Typography variant="h5">{inventoryId}</Typography>
                                                      </div>
                                                  </ItemTooltip>
                                                  {groups.map((group, gi) => {
                                                      return (
                                                          <Timeline key={`${gi}-${inventoryId}`}>
                                                              {group.map((sitem, itemIndex) => {
                                                                  const getLevel = (_item) => {
                                                                      return _get(
                                                                          _get(_item, "properties", []).filter(
                                                                              (_p) => _p.name === "Level"
                                                                          ),
                                                                          "[0].values[0][0]",
                                                                          null
                                                                      );
                                                                  };
                                                                  const getQuality = (_item) => {
                                                                      return _get(
                                                                          _get(_item, "properties", []).filter(
                                                                              (_p) => _p.name === "Quality"
                                                                          ),
                                                                          "[0].values[0][0]",
                                                                          null
                                                                      );
                                                                  };
                                                                  const level = _get(/^(\d+)/.exec(getLevel(sitem)), 1, null);
                                                                  const quality = _get(
                                                                      /^\+(\d+)%/.exec(getQuality(sitem)),
                                                                      1,
                                                                      null
                                                                  );
                                                                  return (
                                                                      <MyTimelineItem key={`${itemIndex}-${sitem.id}`}>
                                                                          <TimelineSeparator>
                                                                              <ItemTooltip
                                                                                  placement="left"
                                                                                  title={<ItemDetail data={sitem} />}
                                                                              >
                                                                                  <img
                                                                                      src={_get(sitem, "icon")}
                                                                                      style={{ width: 32, height: 32 }}
                                                                                  />
                                                                              </ItemTooltip>
                                                                              {itemIndex < group.length - 1 ? (
                                                                                  <TimelineConnector />
                                                                              ) : null}
                                                                          </TimelineSeparator>
                                                                          <TimelineContent>
                                                                              <Typography>{sitem.typeLine}</Typography>
                                                                              {level !== null ? (
                                                                                  <Typography
                                                                                      variant="subtitle2"
                                                                                      color="textSecondary"
                                                                                      style={{ userSelect: "none" }}
                                                                                  >
                                                                                      ({level}
                                                                                      {quality !== null ? (
                                                                                          <span>&nbsp;/&nbsp;{quality}</span>
                                                                                      ) : null}
                                                                                      )
                                                                                  </Typography>
                                                                              ) : null}
                                                                          </TimelineContent>
                                                                      </MyTimelineItem>
                                                                  );
                                                              })}
                                                          </Timeline>
                                                      );
                                                  })}
                                              </Paper>
                                          </Grid>
                                      );
                                  })
                                : null}
                        </Grid>
                        <div style={{ height: "10vh", width: "100%" }} />
                    </div>
                </Container>
            </React.Fragment>
        );
    }
}

const connectedComponent = connect(
    (state) => {
        return { ...state.character, ...state.serverEnv, leagueEntryCount: state.league.entries.length };
    },
    CharacterStore.actionCreators,
    null,
    { forwardRef: true }
)(CharacterPage);

export default withRouter(connectedComponent);
