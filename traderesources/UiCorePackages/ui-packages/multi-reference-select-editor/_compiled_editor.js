var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _possibleConstructorReturn(self, call) { if (!self) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return call && (typeof call === "object" || typeof call === "function") ? call : self; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

var MultiReferenceSelectEditor = function (_React$Component) {
    _inherits(MultiReferenceSelectEditor, _React$Component);

    function MultiReferenceSelectEditor(props) {
        _classCallCheck(this, MultiReferenceSelectEditor);

        var _this = _possibleConstructorReturn(this, (MultiReferenceSelectEditor.__proto__ || Object.getPrototypeOf(MultiReferenceSelectEditor)).call(this, props));

        var selectedValuesDic = {};
        for (var i = 0; i < _this.props.selectedValues.length; i++) {
            selectedValuesDic[_this.props.selectedValues[i]] = true;
        }

        _this.state = {
            selectedValuesDic: selectedValuesDic,
            checkStylesDic: _this.computeCheckStylesDic(_this.props.refItems, selectedValuesDic),
            readonly: _this.props.readonly
        };
        _this.computeCheckStylesDic = _this.computeCheckStylesDic.bind(_this);
        _this.computeCheckStylesArr = _this.computeCheckStylesArr.bind(_this);
        _this.toggleValue = _this.toggleValue.bind(_this);
        _this.renderItems = _this.renderItems.bind(_this);
        _this.flatten = _this.flatten.bind(_this);
        _this.getGlyphIconClass = _this.getGlyphIconClass.bind(_this);
        _this.flattenedRefItems = _this.flatten(_this.props.refItems);
        return _this;
    }

    _createClass(MultiReferenceSelectEditor, [{
        key: "flatten",
        value: function flatten(refItems) {
            var ret = {};
            for (var i = 0; i < refItems.length; i++) {
                var curItem = refItems[i];
                ret[curItem.Code] = curItem;
                $.extend(ret, this.flatten(curItem.Items));
            }
            return ret;
        }
    }, {
        key: "computeCheckStylesDic",
        value: function computeCheckStylesDic(refItems, selectedValuesDic) {
            var checkStyles = this.computeCheckStylesArr(refItems, selectedValuesDic);
            var ret = {};
            for (var i = 0; i < checkStyles.length; i++) {
                ret[checkStyles[i].Code] = checkStyles[i].CheckStyle;
            }
            return ret;
        }
    }, {
        key: "computeCheckStylesArr",
        value: function computeCheckStylesArr(refItems, selectedValuesDic) {
            var ret = [];
            for (var i = 0; i < refItems.length; i++) {
                var curItem = refItems[i];
                if ((curItem.Items || []).length === 0) {
                    if (selectedValuesDic[curItem.Code]) {
                        ret.push({ Code: curItem.Code, CheckStyle: "Checked" });
                    } else {
                        ret.push({ Code: curItem.Code, CheckStyle: "Unchecked" });
                    }
                } else {
                    var childrenCheckStyles = this.computeCheckStylesArr(curItem.Items, selectedValuesDic);
                    var curItemCheckStyle;
                    if (childrenCheckStyles.every(function (x) {
                        return x.CheckStyle === "Checked";
                    })) {
                        curItemCheckStyle = "Checked";
                    } else if (childrenCheckStyles.every(function (x) {
                        return x.CheckStyle === "Unchecked";
                    })) {
                        curItemCheckStyle = "Unchecked";
                    } else {
                        curItemCheckStyle = "SemiChecked";
                    }
                    ret.push({ Code: curItem.Code, CheckStyle: curItemCheckStyle });
                    for (var childrenIndex = 0; childrenIndex < childrenCheckStyles.length; childrenIndex++) {
                        ret.push(childrenCheckStyles[childrenIndex]);
                    }
                }
            }
            return ret;
        }
    }, {
        key: "toggleValue",
        value: function toggleValue(value) {
            if (!this.state.readonly) {
                var selectedValuesDic = $.extend(true, {}, this.state.selectedValuesDic);

                var valuesToChange = [];
                if (this.flattenedRefItems[value].Items.length === 0) {
                    valuesToChange.push(value);
                } else {
                    var leaves = this.getAllLeafItemsCodes(this.flattenedRefItems[value].Items);
                    for (var i = 0; i < leaves.length; i++) {
                        valuesToChange.push(leaves[i]);
                    }
                }
                var curCheckStyle = this.state.checkStylesDic[value];
                var checked = false;
                if (curCheckStyle === "Unchecked" || curCheckStyle === "SemiChecked") {
                    checked = true;
                }

                for (var ind = 0; ind < valuesToChange.length; ind++) {
                    selectedValuesDic[valuesToChange[ind]] = checked;
                }

                this.setState({
                    selectedValuesDic: selectedValuesDic,
                    checkStylesDic: this.computeCheckStylesDic(this.props.refItems, selectedValuesDic)
                });
            }
        }
    }, {
        key: "getAllLeafItemsCodes",
        value: function getAllLeafItemsCodes(refItems) {
            var ret = [];
            for (var i = 0; i < refItems.length; i++) {
                var curItem = refItems[i];
                if (curItem.Items.length === 0) {
                    ret.push(curItem.Code);
                } else {
                    var leaves = this.getAllLeafItemsCodes(curItem.Items);
                    for (var childIndex = 0; childIndex < leaves.length; childIndex++) {
                        ret.push(leaves[childIndex]);
                    }
                }
            }
            return ret;
        }
    }, {
        key: "getGlyphIconClass",
        value: function getGlyphIconClass(checkStyle) {
            switch (checkStyle) {
                case "Unchecked":
                    return "uil-square-full";
                case "Checked":
                    return "uil-check-square";
                case "SemiChecked":
                    return "uil-check";
                default:
                    throw "Not implemented";
            }
        }
    }, {
        key: "getCheckedCodes",
        value: function getCheckedCodes(selectedValuesDic) {
            var ret = [];
            for (var key in selectedValuesDic) {
                if (selectedValuesDic[key] === true) {
                    ret.push(key);
                }
            }
            return ret;
        }
    }, {
        key: "renderItems",
        value: function renderItems(items) {
            var _this2 = this;

            if (items.length === 0) {
                return null;
            }
            return React.createElement(
                "ul",
                { className: "ref-items" },
                items.map(function (item) {
                    return React.createElement(
                        "li",
                        null,
                        React.createElement(
                            "span",
                            { className: _this2.getGlyphIconClass(_this2.state.checkStylesDic[item.Code]), onClick: function onClick(e) {
                                    return _this2.toggleValue(item.Code);
                                }, "aria-hidden": "true" },
                            " "
                        ),
                        React.createElement(
                            "span",
                            { onClick: function onClick(e) {
                                    return _this2.toggleValue(item.Code);
                                }, className: "checkstyle-" + _this2.state.checkStylesDic[item.Code] },
                            item.Text
                        ),
                        " ",
                        _this2.renderItems(item.Items)
                    );
                })
            );
        }
    }, {
        key: "render",
        value: function render() {
            return React.createElement(
                "div",
                null,
                this.renderItems(this.props.refItems),
                React.createElement("input", { type: "hidden", value: JSON.stringify(this.getCheckedCodes(this.state.selectedValuesDic)), name: this.props.name })
            );
        }
    }]);

    return MultiReferenceSelectEditor;
}(React.Component);

$(document).ready(function () {
    $(".multi-reference-select-editor").each(function () {
        var $this = $(this);
        var editorName = $this.data("editorName");
        var isReadonly = $this.data("readonly");
        var items = $this.data("items");
        var selectedValues = $this.data("selectedValues");
        ReactDOM.render(React.createElement(MultiReferenceSelectEditor, { name: editorName, readonly: isReadonly, refItems: items, selectedValues: selectedValues }), this);
    });
});