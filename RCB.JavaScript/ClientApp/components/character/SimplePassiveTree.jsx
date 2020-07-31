import * as React from "react";
import TreeData from "../../treeData";
import { isNode } from "@Utils";

const treeData = TreeData.getRaw();

const m_pi = Math.PI;
const m_sin = Math.sin;
const m_cos = Math.cos;
const m_sqrt = Math.sqrt;

function r2d(radian) {
    return radian * (180 / Math.PI);
}

const SvgCircle = ({ cx, cy, r, ...otherProps }) => {
    return (
        <path
            d={`
      M ${cx - r}, ${cy}
      a ${r},${r} 0 1,0 ${r * 2},0
      a ${r},${r} 0 1,0 ${-1 * r * 2},0
    `}
            {...otherProps}
        />
    );
};

// TODO
// migrate to react-konva
class SimplePassiveTree extends React.PureComponent {
    constructor(props) {
        super(props);
        this.state = {
            _isFirstRender: true,
            offsetX: 0,
            offsetY: 0,
            zoomLevel: 1
        };
    }

    componentDidMount() {
        if (this.state._isFirstRender) {
            this._timer = setTimeout(() => {
                this.setState({ _isFirstRender: false });
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
        const skipRenderOnNode =
            this.props.skipRenderOnNode !== null && this.props.skipRenderOnNode !== undefined
                ? this.props.skipRenderOnNode
                : true;
        const skipFirstRender =
            this.props.skipFirstRender !== null && this.props.skipFirstRender !== undefined ? this.props.skipFirstRender : true;
        if ((skipRenderOnNode && isNode()) || (skipFirstRender && this.state._isFirstRender)) {
            const loading = this.props.loading || "Loading Passive Tree......";
            return loading;
        }
        const charNodes = (this.props.data || "").split(",");
        const orbit4Angle = [
            0,
            10,
            20,
            30,
            40,
            45,
            50,
            60,
            70,
            80,
            90,
            100,
            110,
            120,
            130,
            135,
            140,
            150,
            160,
            170,
            180,
            190,
            200,
            210,
            220,
            225,
            230,
            240,
            250,
            260,
            270,
            280,
            290,
            300,
            310,
            315,
            320,
            330,
            340,
            350
        ];
        const orbitMult = [0, m_pi / 3, m_pi / 6, m_pi / 6];
        const orbitMultFull = [
            0,
            (10 * m_pi) / 180,
            (20 * m_pi) / 180,
            (30 * m_pi) / 180,
            (40 * m_pi) / 180,
            (45 * m_pi) / 180,
            (50 * m_pi) / 180,
            (60 * m_pi) / 180,
            (70 * m_pi) / 180,
            (80 * m_pi) / 180,
            (90 * m_pi) / 180,
            (100 * m_pi) / 180,
            (110 * m_pi) / 180,
            (120 * m_pi) / 180,
            (130 * m_pi) / 180,
            (135 * m_pi) / 180,
            (140 * m_pi) / 180,
            (150 * m_pi) / 180,
            (160 * m_pi) / 180,
            (170 * m_pi) / 180,
            (180 * m_pi) / 180,
            (190 * m_pi) / 180,
            (200 * m_pi) / 180,
            (210 * m_pi) / 180,
            (220 * m_pi) / 180,
            (225 * m_pi) / 180,
            (230 * m_pi) / 180,
            (240 * m_pi) / 180,
            (250 * m_pi) / 180,
            (260 * m_pi) / 180,
            (270 * m_pi) / 180,
            (280 * m_pi) / 180,
            (290 * m_pi) / 180,
            (300 * m_pi) / 180,
            (310 * m_pi) / 180,
            (315 * m_pi) / 180,
            (320 * m_pi) / 180,
            (330 * m_pi) / 180,
            (340 * m_pi) / 180,
            (350 * m_pi) / 180
        ];
        const orbitDist = [0, 82, 162, 335, 493];
        const nodeCircles = [];
        const nodeMap = {};
        for (const k in treeData.nodes) {
            const node = treeData.nodes[k];
            node.id = node.skill;
            node.g = typeof node.group === "number" ? node.group : node.g;
            node.o = node.orbit;
            node.oidx = node.orbitIndex;
            node.dn = node.name;
            node.sd = node.stats;
            node.passivePointsGranted = node.grantedPassivePoints || 0;
            nodeMap[node.id] = node;

            // Determine node type
            if (node.classStartIndex) {
                node.type = "ClassStart";
            } else if (node.isAscendancyStart) {
                node.type = "AscendClassStart";
            } else if (node.m || node.isMastery) {
                node.type = "Mastery";
            } else if (node.isJewelSocket) {
                node.type = "Socket";
            } else if (node.ks || node.isKeystone) {
                node.type = "Keystone";
            } else if (node["not"] || node.isNotable) {
                node.type = "Notable";
            } else {
                node.type = "Normal";
            }

            const group = treeData.groups[parseInt(node.g) - 1];
            if (group) {
                node.group = group;
                group.ascendancyName = node.ascendancyName;
                if (node.isAscendancyStart) {
                    group.isAscendancyStart = true;
                }
                if (node.o != 4) {
                    node.angle = node.oidx * orbitMult[node.o];
                } else {
                    node.angle = orbitMultFull[node.oidx];
                }
                const dist = orbitDist[node.o];
                node.x = group.x + m_sin(node.angle) * dist;
                node.y = group.y - m_cos(node.angle) * dist;
                const fill = charNodes.includes(`${node.id}`) ? "red" : "blue";
                const circleRadius = node.isNotable || node.isKeystone ? 40 : 25;
                const nodeCircle = <circle key={node.id} cx={node.x} cy={node.y} r={circleRadius} fill={fill} />;
                nodeCircles.push(nodeCircle);
            }
        }
        const paths = [];
        for (const k in treeData.nodes) {
            const node = treeData.nodes[k];
            for (const otherId of node.out || []) {
                const other = nodeMap[otherId];
                node.linkedId = otherId;
                other.linkedId = node.id;
                const stroke = charNodes.includes(`${node.id}`) && charNodes.includes(`${other.id}`) ? "red" : "blue";
                const strokeWidth = stroke === "red" ? 20 : 12;
                if (
                    node.type != "ClassStart" &&
                    other.type != "ClassStart" &&
                    node.type != "Mastery" &&
                    other.type != "Mastery" &&
                    node.ascendancyName == other.ascendancyName &&
                    !node.isProxy &&
                    !other.isProxy &&
                    !node.group.isProxy &&
                    !other.group.isProxy &&
                    !isNaN(node.x) &&
                    !isNaN(node.y) &&
                    !isNaN(other.x) &&
                    !isNaN(other.y)
                ) {
                    let node1 = node;
                    let node2 = other;
                    if (node1.g == node2.g && node1.o == node2.o) {
                        if (node1.angle > node2.angle) {
                            [node1, node2] = [node2, node1];
                        }
                        let arcAngle = node2.angle - node1.angle;
                        if (arcAngle > m_pi) {
                            [node1, node2] = [node2, node1];
                            arcAngle = m_pi * 2 - arcAngle;
                        }
                        if (arcAngle < m_pi * 0.9) {
                            const clipAngle = m_pi / 4 - arcAngle / 2;
                            const xdist = Math.abs(node2.x - node1.x);
                            const ydist = Math.abs(node2.y - node1.y);
                            const r = xdist > ydist ? xdist : ydist;
                            const arc = (
                                <path
                                    key={`${node1.x}${node1.y}${node2.x}${node2.y}`}
                                    d={`M${node2.x} ${node2.y} A${r} ${r} 0 0 0 ${node1.x} ${node1.y}`}
                                    stroke={stroke}
                                    fill="transparent"
                                    strokeWidth={strokeWidth}
                                />
                            );
                            paths.push(arc);
                        }
                    } else {
                        const path = (
                            <line
                                key={`${node.x}${node.y}${other.x}${other.y}`}
                                x1={`${node.x}`}
                                y1={`${node.y}`}
                                x2={`${other.x}`}
                                y2={`${other.y}`}
                                stroke={stroke}
                                strokeWidth={strokeWidth}
                            />
                        );
                        paths.push(path);
                    }
                }
            }
        }
        const { offsetX, offsetY, zoomLevel } = this.state;
        const minX = (-7334.32 + offsetX) / zoomLevel;
        const minY = (-7334.32 + offsetY) / zoomLevel;
        const maxX = 14841.83 / zoomLevel;
        const maxY = maxX;
        return (
            <svg
                viewBox={`${minX} ${minY} ${maxX} ${maxY}`}
                style={{ backgroundColor: "#131313" }}
            >
                {paths}
                {nodeCircles}
            </svg>
        );
    }
}

export default SimplePassiveTree;
