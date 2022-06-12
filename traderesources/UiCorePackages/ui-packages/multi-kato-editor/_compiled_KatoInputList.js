var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _possibleConstructorReturn(self, call) { if (!self) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return call && (typeof call === "object" || typeof call === "function") ? call : self; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

var KatoInputList = function (_React$Component) {
    _inherits(KatoInputList, _React$Component);

    function KatoInputList(props) {
        _classCallCheck(this, KatoInputList);

        var _this = _possibleConstructorReturn(this, (KatoInputList.__proto__ || Object.getPrototypeOf(KatoInputList)).call(this, props));

        _this.state = {
            asyncRequestRunning: false,
            asyncRequestId: 0,
            asyncRequestError: null,
            inputs: _this.props.inputs,
            country: _this.props.kato.filter(function (x) {
                return x.ParentId === null;
            }),
            obl: _this.props.kato,
            region: _this.props.kato
        };
        _this.onChange = _this.onChange.bind(_this);
        _this.onSave = _this.onSave.bind(_this);
        _this.validate = _this.validate.bind(_this);
        _this.isNull = _this.isNull.bind(_this);
        _this.isNullOrEmpty = _this.isNullOrEmpty.bind(_this);
        _this.validateRanges = _this.validateRanges.bind(_this);
        return _this;
    }

    _createClass(KatoInputList, [{
        key: "onChange",
        value: function onChange(name, val) {
            var _this2 = this;

            var inputs = $.extend(true, [], this.state.inputs);
            var input = inputs.find(function (x) {
                return x.Name === name;
            });
            input.Value = val;
            input.Error = this.validate(input);
            var generalError = this.state.generalError;
            if (inputs.every(function (input) {
                return _this2.isNullOrEmpty(input.Error);
            })) {
                generalError = null;
            }
            var obls = this.state.obl;
            var regions = this.state.region;
            if (name === "AdrCountry" && val !== null) {
                obls = this.props.kato.filter(function (x) {
                    return x.ParentId === val;
                }).sort();
            }
            if (name === "AdrObl" && val !== null) {
                regions = this.props.kato.filter(function (x) {
                    return x.ParentId === val;
                }).sort();
            }
            this.setState({ inputs: inputs, generalError: generalError, obl: obls, region: regions });
        }
    }, {
        key: "onSave",
        value: function onSave() {
            var _this3 = this;

            var inputs = $.extend(true, [], this.state.inputs);

            inputs.forEach(function (input) {
                input.Error = _this3.validate(input);
            });
            this.validateRanges(function (input) {
                return input.Name;
            }, inputs.filter(function (input) {
                return _this3.isNullOrEmpty(input.Error);
            }));
            this.validateRanges(function (input) {
                return input.Alias;
            }, inputs.filter(function (input) {
                return _this3.isNullOrEmpty(input.Error) && !_this3.isNullOrEmpty(input.Alias);
            }));

            if (inputs.some(function (input) {
                return !_this3.isNullOrEmpty(input.Error);
            })) {
                this.setState({
                    inputs: inputs, generalError: inputs.map(function (input) {
                        if (!_this3.isNullOrEmpty(input.Error)) {
                            return input.Text + ": " + input.Error + "; ";
                        }
                    }) });
            } else {
                this.props.onSave(inputs);
            }
        }
    }, {
        key: "validateRanges",
        value: function validateRanges(getInputNameLambda, inputs) {
            var fromSuffix = "From";
            var toSuffix = "To";

            var froms = inputs.filter(function (input) {
                return getInputNameLambda(input).endsWith(fromSuffix);
            });
            froms.forEach(function (from) {
                var fromInputName = getInputNameLambda(from);
                var inputToName = fromInputName.substring(0, fromInputName.length - fromSuffix.length) + toSuffix;
                var inputTo = inputs.find(function (input) {
                    return getInputNameLambda(input) === inputToName;
                });

                if (inputTo) {
                    if (inputTo.Value < from.Value) {
                        inputTo.Error = "Значение должно быть больше, чем \"" + from.Text + "\"";
                    }
                }
            });
        }
    }, {
        key: "isNull",
        value: function isNull(val) {
            if (val === undefined || val === null) {
                return true;
            }
            return false;
        }
    }, {
        key: "isNullOrEmpty",
        value: function isNullOrEmpty(val) {
            if (this.isNull(val)) {
                return true;
            }
            return val === "";
        }
    }, {
        key: "validate",
        value: function validate(input) {
            if (input.InputType === "File") {
                return null;
            }
            if (!input.IsRequired) {
                return null;
            }

            if (!input.Value && !input.IsHidden) {
                return "поле обязательно для заполнения";
            }

            return null;
        }
    }, {
        key: "render",
        value: function render() {
            var _this4 = this;

            if (!this.props.show) {
                return null;
            }

            var Modal = ReactBootstrap.Modal;
            var Button = ReactBootstrap.Button;
            var inputs = this.state.inputs;
            var country = inputs.find(function (x) {
                return x.Name === "AdrCountry";
            });
            var obl = inputs.find(function (x) {
                return x.Name === "AdrObl";
            });
            var region = inputs.find(function (x) {
                return x.Name === "AdrReg";
            });
            var adr = inputs.find(function (x) {
                return x.Name === "AdrAdr";
            });
            var adrKz = inputs.find(function (x) {
                return x.Name === "AdrAdrKz";
            });
            var phone = inputs.find(function (x) {
                return x.Name === "Phone";
            });
            var email = inputs.find(function (x) {
                return x.Name === "Email";
            });
            var kadNumber = inputs.find(function (x) {
                return x.Name === "CadNumber";
            });
            var generalErrorPanel = null;
            if (this.state.generalError) {
                generalErrorPanel = React.createElement(
                    "div",
                    { className: "alert alert-danger", role: "alert" },
                    this.state.generalError
                );
            }
            var emptyOption = React.createElement(
                "option",
                null,
                "\u0412\u044B\u0431\u0435\u0440\u0438\u0442\u0435 \u0437\u043D\u0430\u0447\u0435\u043D\u0438\u0435"
            );
            var cadNumberInput = null;
            var adrKzInput = null;
            var phoneInput = null;
            var emailInput = null;
            if (phone !== undefined) {
                phoneInput = React.createElement(
                    "div",
                    { className: "form-group" },
                    React.createElement(
                        "label",
                        null,
                        "\u0422\u0435\u043B\u0435\u0444\u043E\u043D"
                    ),
                    React.createElement("input", { className: "form-control", onChange: function onChange(e) {
                            return _this4.onChange(phone.Name, e.target.value);
                        }, value: phone.Value, disabled: this.props.blocked, name: "phone", key: "phone" })
                );
            }
            if (email !== undefined) {
                emailInput = React.createElement(
                    "div",
                    { className: "form-group" },
                    React.createElement(
                        "label",
                        null,
                        "E-mail"
                    ),
                    React.createElement("input", { className: "form-control", onChange: function onChange(e) {
                            return _this4.onChange(email.Name, e.target.value);
                        }, value: email.Value, disabled: this.props.blocked, name: "email", key: "email" })
                );
            }
            if (adrKz !== undefined) {
                adrKzInput = React.createElement(
                    "div",
                    { className: "form-group" },
                    React.createElement(
                        "label",
                        null,
                        "\u0410\u0434\u0440\u0435\u0441 (\u043D\u0430 \u043A\u0430\u0437\u0430\u0445\u0441\u043A\u043E\u043C)"
                    ),
                    React.createElement("input", { className: "form-control", onChange: function onChange(e) {
                            return _this4.onChange(adrKz.Name, e.target.value);
                        }, value: adrKz.Value, disabled: this.props.blocked, name: "AdrAdrKz", key: "AdrAdrKz" })
                );
            }
            if (kadNumber !== undefined) {
                cadNumberInput = React.createElement(
                    "div",
                    { className: "form-group" },
                    React.createElement(
                        "label",
                        null,
                        "\u041A\u0430\u0434\u0430\u0441\u0442\u0440\u043E\u0432\u044B\u0439 \u043D\u043E\u043C\u0435\u0440 \u043E\u0431\u044A\u0435\u043A\u0442\u0430 \u043D\u0435\u0434\u0432\u0438\u0436\u0438\u043C\u043E\u0441\u0442\u0438, \u043D\u0430 \u043A\u043E\u0442\u043E\u0440\u043E\u043C \u0440\u0430\u0441\u043F\u043E\u043B\u043E\u0436\u0435\u043D \u0441\u0443\u0431\u044A\u0435\u043A\u0442 \u0430\u043A\u043A\u0440\u0435\u0434\u0438\u0442\u0430\u0446\u0438\u0438"
                    ),
                    React.createElement(SearchCollectionEditor, { twoFielded: true, name: kadNumber.Name, dontPutHiddenInput: true, onChange: function onChange(e) {
                            return _this4.onChange(kadNumber.Name, e);
                        }, value: kadNumber.Value, searchCollectionUrl: kadNumber.ModelCollectionUrl, searchDialogHeaderText: "\u0412\u044B\u0431\u043E\u0440", queryInputPlaceholder: "\u0411\u0418\u041D \u043E\u0440\u0433\u0430\u043D\u0438\u0437\u0430\u0446\u0438\u0438;\u041A\u0430\u0434\u0430\u0441\u0442\u0440\u043E\u0432\u044B\u0439 \u043D\u043E\u043C\u0435\u0440", key: kadNumber.Name })
                );
            }
            return React.createElement(
                "div",
                null,
                React.createElement(
                    Modal,
                    { show: true, backdrop: "static", onHide: this.props.onCancel },
                    React.createElement(
                        Modal.Header,
                        { closeButton: true },
                        React.createElement(
                            Modal.Title,
                            { id: "contained-modal-title-lg" },
                            this.props.text
                        )
                    ),
                    React.createElement(
                        Modal.Body,
                        null,
                        generalErrorPanel,
                        React.createElement(
                            "div",
                            null,
                            React.createElement(
                                "div",
                                { className: "form-group" },
                                React.createElement(
                                    "label",
                                    null,
                                    "\u0421\u0442\u0440\u0430\u043D\u0430"
                                ),
                                React.createElement(
                                    "select",
                                    { className: "form-control", onChange: function onChange(e) {
                                            return _this4.onChange("AdrCountry", e.target.value);
                                        }, value: country.Value, disabled: this.props.blocked, name: "AdrCountry", key: "AdrCountry" },
                                    emptyOption,
                                    this.state.country !== null ? this.state.country.map(function (i) {
                                        return React.createElement(
                                            "option",
                                            { value: i.Id },
                                            i.Text
                                        );
                                    }) : ""
                                )
                            ),
                            React.createElement(
                                "div",
                                { className: "form-group" },
                                React.createElement(
                                    "label",
                                    null,
                                    "\u041E\u0431\u043B\u0430\u0441\u0442\u044C"
                                ),
                                React.createElement(
                                    "select",
                                    { className: "form-control", onChange: function onChange(e) {
                                            return _this4.onChange("AdrObl", e.target.value);
                                        }, value: obl.Value, disabled: this.props.blocked, name: "AdrObl", key: "AdrObl" },
                                    emptyOption,
                                    this.state.obl !== null ? this.state.obl.map(function (i) {
                                        return React.createElement(
                                            "option",
                                            { value: i.Id },
                                            i.Text
                                        );
                                    }) : ""
                                )
                            ),
                            React.createElement(
                                "div",
                                { className: "form-group" },
                                React.createElement(
                                    "label",
                                    null,
                                    "\u0420\u0435\u0433\u0438\u043E\u043D"
                                ),
                                React.createElement(
                                    "select",
                                    { className: "form-control", onChange: function onChange(e) {
                                            return _this4.onChange("AdrReg", e.target.value);
                                        }, value: region.Value, disabled: this.props.blocked, name: "AdrReg", key: "AdrReg" },
                                    emptyOption,
                                    this.state.region !== null ? this.state.region.map(function (i) {
                                        return React.createElement(
                                            "option",
                                            { value: i.Id },
                                            i.Text
                                        );
                                    }) : ""
                                )
                            ),
                            React.createElement(
                                "div",
                                { className: "form-group" },
                                React.createElement(
                                    "label",
                                    null,
                                    "\u0410\u0434\u0440\u0435\u0441"
                                ),
                                React.createElement("input", { className: "form-control", onChange: function onChange(e) {
                                        return _this4.onChange(adr.Name, e.target.value);
                                    }, value: adr.Value, disabled: this.props.blocked, name: "AdrAdr", key: "AdrAdr" })
                            ),
                            phoneInput,
                            emailInput,
                            adrKzInput,
                            cadNumberInput
                        )
                    ),
                    React.createElement(
                        Modal.Footer,
                        null,
                        React.createElement(
                            Button,
                            { onClick: this.onSave },
                            "\u0414\u043E\u0431\u0430\u0432\u0438\u0442\u044C"
                        ),
                        React.createElement(
                            Button,
                            { onClick: this.props.onCancel },
                            "\u041E\u0442\u043C\u0435\u043D\u0430"
                        )
                    )
                )
            );
        }
    }]);

    return KatoInputList;
}(React.Component);