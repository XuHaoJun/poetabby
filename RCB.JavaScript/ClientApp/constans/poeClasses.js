export const baseClasses = ["Scion", "Marauder", "Duelist", "Ranger", "Shadow", "Witch", "Templar"].sort();
export const ascendancyClasses = [
    "Juggernaut",
    "Berserker",
    "Chieftain",
    "Slayer",
    "Gladiator",
    "Champion",
    "Deadeye",
    "Raider",
    "Pathfinder",
    "Assassin",
    "Saboteur",
    "Trickster",
    "Necromancer",
    "Occultist",
    "Elementalist",
    "Inquisitor",
    "Hierophant",
    "Guardian",
    "Ascendant"
].sort();

export const poeClasses = {
    base: baseClasses,
    ascendancy: ascendancyClasses,
    all: ascendancyClasses.concat(baseClasses)
};

export default poeClasses;
