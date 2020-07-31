import * as React from "react";
import treeData from "../treeData/tree_3_11.json";

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

class PassiveTreePage extends React.Component {
    render() {
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
        const charNodes = "487,885,1031,1203,2292,3533,3644,4177,4184,4397,5197,5233,5415,5743,5935,6289,6712,6764,6770,7162,7374,7388,7938,8198,9386,9695,10031,10893,11088,11420,11490,11730,12246,13009,13164,14021,14040,14057,14930,14936,17579,17735,17821,18715,19103,21330,21934,21958,21974,22088,23027,23090,23509,23572,24324,25831,26196,26270,26298,26481,26712,27203,27323,29061,29199,29353,30380,31875,32932,33287,33296,33435,33755,34661,35260,35288,36017,36634,36774,36949,37999,39648,40291,40366,40766,41190,41472,42009,42760,43061,44184,44202,44429,44799,44967,48287,48362,49254,50422,53118,54694,55190,55332,55676,55993,57264,58218,59928,60398,60501,61804,63282,63723,64210,64587,65108".split(
            ","
        );
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
                const nodeCircle = <circle key={node.id} cx={node.x} cy={node.y} r={40} fill={fill} />;
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
                                    strokeWidth={3}
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
                                strokeWidth={3}
                            />
                        );
                        paths.push(path);
                    }
                }
            }
        }
        return (
            <svg viewBox="-7334.32 -7334.32 14841.83 14841.83">
                {paths}
                {nodeCircles}
            </svg>
        );
    }
}

export default PassiveTreePage;
