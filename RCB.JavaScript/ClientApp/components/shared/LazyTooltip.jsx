import * as React from "react";

import Tooltip from "@material-ui/core/Tooltip";

export default function LazyTooltip(props) {
    const [isOpen, setIsOpen] = React.useState(false);
    const { title: _title, renderTitle, placeholder, onOpen, children, ...otherProps } = props;
    var title;
    if (_title) {
        title = _title;
    } else if (renderTitle && isOpen) {
        const state = { isOpen };
        title = renderTitle(props, state);
    } else {
        title = placeholder || "";
    }
    return (
        <Tooltip
            title={title}
            onOpen={(e) => {
                setIsOpen(true);
                if (onOpen) {
                    onOpen(e);
                }
            }}
            {...otherProps}
        >
            {children}
        </Tooltip>
    );
}
