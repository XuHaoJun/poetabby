import NormalDivider from "./divider_normal.png";
import MagicDivider from "./divider_magic.png";
import RareDivider from "./divider_rare.png";
import UniqueDivider from "./divider_unique.png";
import GemDivider from "./divider_gem.png";

export const Normal = NormalDivider;
export const Magic = MagicDivider;
export const Rare = RareDivider;
export const Unique = UniqueDivider;
export const Gem = GemDivider;

export const Currency = NormalDivider;

export const getDividerByFrameType = (frameType = 0) => {
    if (frameType == 0) {
        return Normal;
    } else if (frameType == 1) {
        return Magic;
    } else if (frameType == 2) {
        return Rare;
    } else if (frameType == 3) {
        return Unique;
    } else if (frameType == 4) {
        return Gem;
    } else {
        return Normal;
    }
};
