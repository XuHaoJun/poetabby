import _last from "lodash/last";

export default class Result {
    value;
    errors;

    get hasErrors() {
        return this.errors != null && Array.isArray(this.errors) && this.errors.length > 0;
    }

    get lastError() {
        return _last(this.errors || []);
    }

    constructor(value, ...errors) {
        this.value = value;
        this.errors = errors[0] == undefined || errors[0] == null ? [] : errors;
    }
}
