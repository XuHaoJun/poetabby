import React from "react";
import Button from "@material-ui/core/Button";
import Menu from "@material-ui/core/Menu";
import MenuItem from "@material-ui/core/MenuItem";

export default function SimpleMenu() {
    const [anchorEl, setAnchorEl] = React.useState(null);

    const handleClick = (event) => {
        setAnchorEl(event.currentTarget);
    };

    const handleClose = () => {
        setAnchorEl(null);
    };

    return (
        <div>
            <Button
                style={{ textTransform: "none" }}
                aria-controls="simple-menu"
                aria-haspopup="true"
                onClick={handleClick}
                variant="contained"
                color="secondary"
            >
                Harvest
            </Button>
            <Menu id="simple-menu" anchorEl={anchorEl} keepMounted open={Boolean(anchorEl)} onClose={handleClose}>
                <MenuItem onClick={handleClose}>Not Implemented!</MenuItem>
                <MenuItem onClick={handleClose}>Harvest</MenuItem>
                <MenuItem onClick={handleClose}>HC Harvest</MenuItem>
                <MenuItem onClick={handleClose}>SSF Harvest</MenuItem>
                <MenuItem onClick={handleClose}>SSF Harvest HC</MenuItem>
            </Menu>
        </div>
    );
}
