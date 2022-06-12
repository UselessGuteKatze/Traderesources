var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _possibleConstructorReturn(self, call) { if (!self) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return call && (typeof call === "object" || typeof call === "function") ? call : self; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

var EducationItemEditor = function (_React$Component) {
    _inherits(EducationItemEditor, _React$Component);

    function EducationItemEditor(props) {
        _classCallCheck(this, EducationItemEditor);

        var _this = _possibleConstructorReturn(this, (EducationItemEditor.__proto__ || Object.getPrototypeOf(EducationItemEditor)).call(this, props));

        _this.state = {
            asyncRequestRunning: false,
            asyncRequestId: 0,
            asyncRequestError: null,
            item: _this.props.item
        };
        _this.onChange = _this.onChange.bind(_this);
        return _this;
    }

    _createClass(EducationItemEditor, [{
        key: "onChange",
        value: function onChange(name, val) {
            var item = $.extend({}, this.state.item, true);
            var input = item.Inputs.find(function (x) {
                return x.Name === name;
            });
            input.Value = val;
            input.VisibleValue = val;
            if (input.InputType === "Reference" || input.InputType === "Reference") {
                input.VisibleValue = input.RefItems.find(function (x) {
                    return x.Code === val;
                }).Text;
            }
            if (input.InputType === "File") {
                input.VisibleValue = input.Value.map(function (i) {
                    return i.FileName;
                }).join(",");
            }
            this.setState({ item: item });
        }
    }, {
        key: "render",
        value: function render() {
            var _this2 = this;

            if (!this.props.show) {
                return null;
            }

            var Modal = ReactBootstrap.Modal;
            var Button = ReactBootstrap.Button;

            return React.createElement(
                "div",
                null,
                React.createElement(
                    Modal,
                    { show: this.props.show, onHide: this.props.onCancel },
                    React.createElement(
                        Modal.Header,
                        null,
                        React.createElement(
                            Modal.Title,
                            { id: "contained-modal-title-lg" },
                            "\u0414\u0430\u043D\u043D\u044B\u0435 \u043E\u0431 \u043E\u0431\u0440\u0430\u0437\u043E\u0432\u0430\u043D\u0438\u0438"
                        )
                    ),
                    React.createElement(
                        Modal.Body,
                        null,
                        React.createElement(
                            "div",
                            null,
                            this.state.item.Inputs.map(function (i) {
                                return React.createElement(
                                    "div",
                                    { className: "form-group", key: i.Id },
                                    React.createElement(
                                        "label",
                                        { htmlFor: "loan-item-name" },
                                        i.Text
                                    ),
                                    React.createElement(EducationInputItem, { item: i, onChange: function onChange(val) {
                                            return _this2.onChange(i.Name, val);
                                        } })
                                );
                            })
                        )
                    ),
                    React.createElement(
                        Modal.Footer,
                        null,
                        React.createElement(
                            Button,
                            { onClick: function onClick(e) {
                                    return _this2.props.onSave(_this2.state.item);
                                } },
                            "\u041E\u041A"
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

    return EducationItemEditor;
}(React.Component);