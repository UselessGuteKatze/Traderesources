var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _possibleConstructorReturn(self, call) { if (!self) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return call && (typeof call === "object" || typeof call === "function") ? call : self; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

var DocsInputItem = function (_React$Component) {
    _inherits(DocsInputItem, _React$Component);

    function DocsInputItem(props) {
        _classCallCheck(this, DocsInputItem);

        return _possibleConstructorReturn(this, (DocsInputItem.__proto__ || Object.getPrototypeOf(DocsInputItem)).call(this, props));
    }

    _createClass(DocsInputItem, [{
        key: "render",
        value: function render() {
            var _this2 = this;

            if (this.props.item.InputType === "Decimal") {
                return React.createElement("input", { className: "form-control", type: "number", onChange: function onChange(e) {
                        return _this2.props.onChange(e.target.valueAsNumber);
                    }, value: this.props.item.Value, disabled: this.props.blocked, name: this.props.item.Name, key: this.props.item.Name });
            }
            if (this.props.item.InputType === "Text") {
                return React.createElement("input", { className: "form-control", onChange: function onChange(e) {
                        return _this2.props.onChange(e.target.value);
                    }, value: this.props.item.Value, disabled: this.props.blocked, name: this.props.item.Name, key: this.props.item.Name });
            }
            if (this.props.item.InputType === "reference") {
                return React.createElement(
                    "select",
                    { className: "form-control", onChange: function onChange(e) {
                            return _this2.props.onChange(e.target.value);
                        }, value: this.props.item.Value, disabled: this.props.blocked, name: this.props.item.Name, key: this.props.item.Name },
                    !this.props.item.Value ? React.createElement(
                        "option",
                        null,
                        "-"
                    ) : null,
                    this.props.item.RefItems.map(function (i) {
                        return React.createElement(
                            "option",
                            { value: i.Code },
                            i.Text
                        );
                    })
                );
            }
            if (this.props.item.InputType === "Boolean") {
                return React.createElement(
                    "select",
                    { className: "form-control", onChange: function onChange(e) {
                            return _this2.props.onChange(e.target.value);
                        }, value: this.props.item.Value, disabled: this.props.blocked, name: this.props.item.Name, key: this.props.item.Name },
                    !this.props.item.Value ? React.createElement(
                        "option",
                        null,
                        "-"
                    ) : null,
                    this.props.item.RefItems.map(function (i) {
                        return React.createElement(
                            "option",
                            { value: i.Code },
                            i.Text
                        );
                    })
                );
            }
            if (this.props.item.InputType === "File") {
                return React.createElement("input", { className: "form-control", type: "file", onChange: function onChange(e) {
                        return _this2.props.onChange(e.target.value);
                    }, value: this.props.item.Value, disabled: this.props.blocked, name: this.props.item.Name, key: this.props.item.Name });
            }

            throw "Не удалось определить редактор данных : " + this.props.item.InputType;
        }
    }]);

    return DocsInputItem;
}(React.Component);