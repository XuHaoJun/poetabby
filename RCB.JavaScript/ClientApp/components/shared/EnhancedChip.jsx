import React from "react";
import clsx from "clsx";

import { withStyles } from "@material-ui/core/styles";
import Chip from "@material-ui/core/Chip";
import { styles as chipStyles } from "@material-ui/core/Chip/Chip";
import capitalize from "@material-ui/core/utils/capitalize";

import _isArray from "lodash/isArray";

function isReactFragment(variableToInspect) {
    if (variableToInspect.type) {
        return variableToInspect.type === React.Fragment;
    }
    return variableToInspect === React.Fragment;
}

export default withStyles(chipStyles)(function EnhancedChip(props) {
    const { variant, size, classes, color, onDelete, deleteIcon, ...others } = props;
    if (onDelete && isReactFragment(deleteIcon) && _isArray(deleteIcon.props.children)) {
        const small = size === "small";
        const customClasses = clsx({
            [classes.deleteIconSmall]: small,
            [classes[`deleteIconColor${capitalize(color)}`]]: color !== "default" && variant !== "outlined",
            [classes[`deleteIconOutlinedColor${capitalize(color)}`]]: color !== "default" && variant === "outlined"
        });

        const deleteIcons = deleteIcon.props.children.map((child, index) => {
            if (React.isValidElement(child)) {
                const handleDeleteIconClick = (event) => {
                    // Stop the event from bubbling up to the `Chip`
                    event.stopPropagation();
                    if (onDelete) {
                        onDelete(event, child);
                    }
                };
                return (
                    <React.Fragment key={index}>
                        {React.cloneElement(child, {
                            className: clsx(child.props.className, classes.deleteIcon, customClasses),
                            onClick: handleDeleteIconClick
                        })}
                    </React.Fragment>
                );
            } else {
                return <React.Fragment key={index}>{child}</React.Fragment>;
            }
        });
        return <Chip {...props} deleteIcon={<React.Fragment>{deleteIcons}</React.Fragment>} />;
    } else {
        return <Chip {...props} />;
    }
});
