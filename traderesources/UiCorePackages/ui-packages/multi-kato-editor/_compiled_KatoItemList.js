var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _possibleConstructorReturn(self, call) { if (!self) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return call && (typeof call === "object" || typeof call === "function") ? call : self; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

var KatoItemList = function (_React$Component) {
    _inherits(KatoItemList, _React$Component);

    function KatoItemList(props) {
        _classCallCheck(this, KatoItemList);

        var _this = _possibleConstructorReturn(this, (KatoItemList.__proto__ || Object.getPrototypeOf(KatoItemList)).call(this, props));

        var items = _this.props.items;
        if (items === undefined || items === null) {
            items = [];
        }
        items = items.map(function (x) {
            var inputs = $.extend(true, [], _this.props.inputs);
            for (var i = 0; i < inputs.length; i++) {
                var input = inputs[i];
                input.Value = x[input.Name];
                _this.prepareInput(input);
            }
            return { Inputs: inputs };
        });
        _this.state = {
            items: items,
            inputs: _this.props.inputs,
            showSelectModal: false,
            editRowIndex: null,
            editRequestId: 0
        };
        _this.prepareInput = _this.prepareInput.bind(_this);
        _this.getDateFromStr = _this.getDateFromStr.bind(_this);
        _this.getEmptyItem = _this.getEmptyItem.bind(_this);
        _this.getItemByRowIndex = _this.getItemByRowIndex.bind(_this);
        _this.showEditor = _this.showEditor.bind(_this);
        _this.closeEditor = _this.closeEditor.bind(_this);
        _this.saveItemAndCloseEditor = _this.saveItemAndCloseEditor.bind(_this);
        return _this;
    }

    _createClass(KatoItemList, [{
        key: "prepareInput",
        value: function prepareInput(input) {
            input.VisibleValue = input.Value;
            if (input.Name === "AdrCountry" || input.Name === "AdrObl" || input.Name === "AdrReg") {
                if (input.VisibleValue !== null) {
                    input.VisibleValue = this.props.kato.find(function (x) {
                        return x.Id === input.Value;
                    }).Text;
                }
            }
            if (input.InputType === "CollectionModel" && input.Value) {
                input.VisibleValue = input.Value.SearchItemText;
            }
            if (input.VisibleValue === null || input.VisibleValue === undefined) {
                input.VisibleValue = "";
            }
        }
    }, {
        key: "getDateFromStr",
        value: function getDateFromStr(newtonsoftJsonSerializedDateString) {
            if (newtonsoftJsonSerializedDateString instanceof Date) {
                return newtonsoftJsonSerializedDateString;
            }
            return new Date(Date.parse(newtonsoftJsonSerializedDateString));
        }
    }, {
        key: "getEmptyItem",
        value: function getEmptyItem() {
            var inputs = $.extend(true, [], this.props.inputs);
            for (var i = 0; i < inputs.length; i++) {
                this.prepareInput(inputs[i]);
            }
            return {
                Inputs: inputs
            };
        }
    }, {
        key: "getItemByRowIndex",
        value: function getItemByRowIndex(rowIndex) {
            if (rowIndex === -1) {
                return this.getEmptyItem();
            } else {
                return this.state.items[rowIndex];
            }
        }
    }, {
        key: "showEditor",
        value: function showEditor(rowIndex) {
            this.setState({ editRowIndex: rowIndex, editRequestId: this.state.editRequestId + 1 });
        }
    }, {
        key: "tryRemove",
        value: function tryRemove(rowIndex) {
            if (confirm("Удалить запись?") !== true) {
                return;
            }
            var items = $.extend(true, [], this.state.items);
            items.splice(rowIndex, 1);
            this.setState({ items: items });
        }
    }, {
        key: "closeEditor",
        value: function closeEditor() {
            this.setState({ editRowIndex: null });
        }
    }, {
        key: "saveItemAndCloseEditor",
        value: function saveItemAndCloseEditor(inputs, rowIndex) {
            var _this2 = this;

            inputs = $.extend(true, [], inputs);
            inputs.forEach(function (input) {
                return _this2.prepareInput(input);
            });
            var items = $.extend(true, [], this.state.items);
            if (rowIndex === -1) {
                items.push({ Inputs: inputs });
            } else {
                items[rowIndex].Inputs = inputs;
            }
            this.setState({ items: items });
            this.closeEditor();
        }
    }, {
        key: "getOpsCellIfNotReadonly",
        value: function getOpsCellIfNotReadonly(rowIndex) {
            var _this3 = this;

            if (this.props.readonly) {
                return null;
            }
            return React.createElement(
                "td",
                null,
                React.createElement(
                    "span",
                    { className: "badge badge-success cursor-pointer", onClick: function onClick(e) {
                            return _this3.showEditor(rowIndex);
                        } },
                    "\u0420\u0435\u0434\u0430\u043A\u0442\u0438\u0440\u043E\u0432\u0430\u0442\u044C"
                ),
                React.createElement(
                    "span",
                    null,
                    "\xA0"
                ),
                React.createElement(
                    "span",
                    { className: "badge badge-danger cursor-pointer", onClick: function onClick(e) {
                            return _this3.tryRemove(rowIndex);
                        } },
                    "\u0423\u0434\u0430\u043B\u0438\u0442\u044C"
                )
            );
        }
    }, {
        key: "render",
        value: function render() {
            var _this4 = this;

            var items = this.state.items.map(function (item, index) {
                return item.Name !== "FullAdr" ? React.createElement(
                    "tr",
                    { key: item.Id, className: "custom-item" },
                    React.createElement(
                        "td",
                        null,
                        index + 1
                    ),
                    _this4.getOpsCellIfNotReadonly(index),
                    React.createElement(
                        "td",
                        null,
                        item.Inputs.map(function (itm) {
                            return itm.Name !== "FullAdr" ? React.createElement(
                                "div",
                                null,
                                React.createElement(
                                    "b",
                                    null,
                                    itm.Text
                                ),
                                ": " + itm.VisibleValue.toString()
                            ) : "";
                        })
                    )
                ) : "";
            });
            var empty = null;
            if (this.state.items.length === 0) {
                empty = React.createElement(
                    "tr",
                    { key: "empty", className: "empty-item" },
                    React.createElement(
                        "td",
                        { colSpan: this.props.readonly === true ? 2 : 3 },
                        "\u041D\u0435\u0442 \u0437\u0430\u043F\u0438\u0441\u0435\u0439"
                    )
                );
            }

            var customItemEditor = null;
            if (this.state.editRowIndex !== null) {
                customItemEditor = React.createElement(KatoInputList, { show: true, key: this.state.editRequestId, text: this.props.text, kato: this.props.kato, inputs: this.getItemByRowIndex(this.state.editRowIndex).Inputs, onCancel: this.closeEditor, onSave: function onSave(inputs) {
                        return _this4.saveItemAndCloseEditor(inputs, _this4.state.editRowIndex);
                    } });
            }
            var val = this.state.items.map(function (x) {
                var ret = {};
                for (var inputIndex = 0; inputIndex < x.Inputs.length; inputIndex++) {
                    var curInput = x.Inputs[inputIndex];
                    ret[curInput.Name] = curInput.Value;
                }
                return ret;
            });
            var addBtn = null;
            var operationsHeaderCell = null;
            if (this.props.readonly !== true) {
                addBtn = React.createElement(
                    "div",
                    null,
                    React.createElement(
                        "button",
                        { className: "btn btn-primary", type: "button", onClick: function onClick(e) {
                                return _this4.showEditor(-1);
                            } },
                        "\u0414\u043E\u0431\u0430\u0432\u0438\u0442\u044C"
                    )
                );
                operationsHeaderCell = React.createElement(
                    "th",
                    { width: "180px" },
                    "\u041E\u043F\u0435\u0440\u0430\u0446\u0438\u0438"
                );
            }
            return React.createElement(
                "div",
                null,
                addBtn,
                React.createElement(
                    "div",
                    null,
                    React.createElement(
                        "table",
                        { className: "table" },
                        React.createElement(
                            "thead",
                            null,
                            React.createElement(
                                "tr",
                                null,
                                React.createElement(
                                    "th",
                                    { width: "20px" },
                                    "\u2116"
                                ),
                                operationsHeaderCell,
                                React.createElement(
                                    "th",
                                    null,
                                    "\u0414\u0430\u043D\u043D\u044B\u0435"
                                )
                            )
                        ),
                        React.createElement(
                            "tbody",
                            null,
                            items,
                            empty
                        )
                    )
                ),
                customItemEditor,
                React.createElement("input", { type: "hidden", value: JSON.stringify(val), name: this.props.name })
            );
        }
    }]);

    return KatoItemList;
}(React.Component);

$(document).ready(function () {
    $(".kato-object-list-editor").each(function () {
        var $this = $(this);
        var editorName = $this.data("editorName");
        var isReadonly = $this.data("readonly");
        var items = $this.data("items");
        var inputs = $this.data("inputs");
        var kato = $this.data("kato");
        var text = $this.data("text");
        ReactDOM.render(React.createElement(KatoItemList, { name: editorName, text: text, readonly: isReadonly, items: items, inputs: inputs, kato: kato }), this);
    });
});