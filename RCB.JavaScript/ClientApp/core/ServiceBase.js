import Result from "./Result";
import Axios, { AxiosRequestConfig } from "axios";
import { transformUrl } from "domain-wait";
import queryString from "query-string";
import _isString from "lodash/isString";
import _has from "lodash/has";
import _get from "lodash/get";

import { isNode, showErrors, getNodeProcess } from "@Utils";
import SessionManager from "./session";

/**
 * Represents base class of the isomorphic service.
 */
export class ServiceBase {
    /**
     * Make request with JSON data.
     * @param opts
     */
    async requestJson(opts) {
        var axiosResult = null;
        var result = null;

        opts.url = transformUrl(opts.url); // Allow requests also for the Node.

        var processQuery = (url, data) => {
            if (data) {
                return `${url}?${queryString.stringify(data)}`;
            }
            return url;
        };

        let axiosRequestConfig;

        if (isNode()) {
            const ssrSessionData = SessionManager.getSessionContext().ssr;
            const { cookie } = ssrSessionData;

            // Make SSR requests 'authorized' from the NodeServices to the web server.
            axiosRequestConfig = {
                headers: {
                    Cookie: cookie
                }
            };
        }

        try {
            switch (opts.method) {
                case "GET":
                    axiosResult = await Axios.get(processQuery(opts.url, opts.data), axiosRequestConfig);
                    break;
                case "POST":
                    axiosResult = await Axios.post(opts.url, opts.data, axiosRequestConfig);
                    break;
                case "PUT":
                    axiosResult = await Axios.put(opts.url, opts.data, axiosRequestConfig);
                    break;
                case "PATCH":
                    axiosResult = await Axios.patch(opts.url, opts.data, axiosRequestConfig);
                    break;
                case "DELETE":
                    axiosResult = await Axios.delete(processQuery(opts.url, opts.data), axiosRequestConfig);
                    break;
            }
            const value = (() => {
                if ((axiosResult.data !== undefined || axiosResult.data !== null) && axiosResult.data.value !== undefined) {
                    return axiosResult.data.value;
                } else {
                    return axiosResult.data;
                }
            })();
            const errors = (() => {
                if (axiosResult.data.errors) {
                    return axiosResult.data.errors;
                } else if (axiosResult.data.error) {
                    return [axiosResult.data.error];
                } else {
                    return [];
                }
            })();
            result = new Result(value, ...errors);
        } catch (error) {
            result = new Result(null, error);
        }

        if (result.hasErrors) {
            showErrors(
                ...result.errors.map((e) => {
                    if (_isString(e)) {
                        return e;
                    } else if (_has(e, "message")) {
                        return e.message;
                    } else {
                        return e;
                    }
                })
            );
        }

        return result;
    }

    /**
     * Allows you to send files to the server.
     * @param opts
     */
    async sendFormData(opts) {
        let axiosResult = null;
        let result = null;

        opts.url = transformUrl(opts.url); // Allow requests also for Node.

        var axiosOpts = {
            headers: {
                "Content-Type": "multipart/form-data"
            }
        };

        try {
            switch (opts.method) {
                case "POST":
                    axiosResult = await Axios.post(opts.url, opts.data, axiosOpts);
                    break;
                case "PUT":
                    axiosResult = await Axios.put(opts.url, opts.data, axiosOpts);
                    break;
                case "PATCH":
                    axiosResult = await Axios.patch(opts.url, opts.data, axiosOpts);
                    break;
            }
            result = new Result(axiosResult.data.value, ...axiosResult.data.errors);
        } catch (error) {
            result = new Result(null, error.message);
        }

        if (result.hasErrors) {
            showErrors(...result.errors);
        }

        return result;
    }
}
