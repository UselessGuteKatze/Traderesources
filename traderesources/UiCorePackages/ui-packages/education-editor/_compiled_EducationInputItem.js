var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _possibleConstructorReturn(self, call) { if (!self) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return call && (typeof call === "object" || typeof call === "function") ? call : self; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

var EducationInputItem = function (_React$Component) {
    _inherits(EducationInputItem, _React$Component);

    function EducationInputItem(props) {
        _classCallCheck(this, EducationInputItem);

        return _possibleConstructorReturn(this, (EducationInputItem.__proto__ || Object.getPrototypeOf(EducationInputItem)).call(this, props));
    }

    _createClass(EducationInputItem, [{
        key: "render",
        value: function render() {
            var _this2 = this;

            if (this.props.item.InputType === "Decimal" || this.props.item.InputType === "Int") {
                return React.createElement("input", { className: "form-control", type: "number", onChange: function onChange(e) {
                        return _this2.props.onChange(e.target.valueAsNumber);
                    }, value: this.props.item.Value, disabled: this.props.blocked, name: this.props.item.Name, key: this.props.item.Name });
            }
            if (this.props.item.InputType === "Text") {
                return React.createElement("input", { className: "form-control", onChange: function onChange(e) {
                        return _this2.props.onChange(e.target.value);
                    }, value: this.props.item.Value, disabled: this.props.blocked, name: this.props.item.Name, key: this.props.item.Name });
            }
            if (this.props.item.InputType === "Reference") {
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
                //return <input className="form-control" type="file" onChange={e => this.props.onChange(e.target.value)} value={this.props.item.Value} disabled={this.props.blocked} name={this.props.item.Name} key={this.props.item.Name} />;
                return React.createElement(FileUploadEditor, { name: this.props.item.Name, files: this.props.item.Value, fileUploadUrl: this.props.item.FileUploadUrl, description: "\u0414\u043E\u043A\u0443\u043C\u0435\u043D\u0442\u044B", onChange: function onChange(e) {
                        return _this2.props.onChange(e.target.value);
                    }, disabled: this.props.blocked });
            }
            if (this.props.item.InputType === "Date") {
                return React.createElement("input", { className: "form-control", type: "date", onChange: function onChange(e) {
                        return _this2.props.onChange(e.target.value);
                    }, value: this.props.item.Value, disabled: this.props.blocked, name: this.props.item.Name, key: this.props.item.Name });
            }

            throw "Не удалось определить редактор данных : " + this.props.item.InputType;
        }
    }]);

    return EducationInputItem;
}(React.Component);