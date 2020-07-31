import * as React from "react";
import { withStyles } from "@material-ui/core/styles";
import CssBaseline from "@material-ui/core/CssBaseline";
import { ToastContainer } from "react-toastify";

const StyledCssBaseline = withStyles((theme) => ({
    "@global": {
        body: {
            backgroundColor: "#E9EBEE"
        }
    }
}))(CssBaseline);

export default class CssbaseLayout extends React.Component {
    render() {
        return (
            <React.Fragment>
                <StyledCssBaseline />
                {this.props.children}
                <ToastContainer />
            </React.Fragment>
        );
    }
}
