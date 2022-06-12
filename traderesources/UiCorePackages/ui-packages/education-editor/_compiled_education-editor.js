var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _possibleConstructorReturn(self, call) { if (!self) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return call && (typeof call === "object" || typeof call === "function") ? call : self; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

var EducationEditor = function (_React$Component) {
    _inherits(EducationEditor, _React$Component);

    function EducationEditor(props) {
        _classCallCheck(this, EducationEditor);

        var _this = _possibleConstructorReturn(this, (EducationEditor.__proto__ || Object.getPrototypeOf(EducationEditor)).call(this, props));

        var items = _this.props.items;
        if (items === undefined || items === null) {
            items = [];
        }

        _this.state = {
            items: items,
            inputs: _this.props.inputs,
            showSelectModal: false,
            editRowIndex: null,
            editRequestId: 0
        };
        _this.getEmptyEducation = _this.getEmptyEducation.bind(_this);
        _this.getItemByRowIndex = _this.getItemByRowIndex.bind(_this);
        _this.showEditor = _this.showEditor.bind(_this);
        _this.closeEditor = _this.closeEditor.bind(_this);
        _this.saveItemAndCloseEditor = _this.saveItemAndCloseEditor.bind(_this);
        return _this;
    }

    _createClass(EducationEditor, [{
        key: "getEmptyEducation",
        value: function getEmptyEducation() {
            return {
                Inputs: this.state.inputs
            };
        }
    }, {
        key: "getItemByRowIndex",
        value: function getItemByRowIndex(rowIndex) {
            if (rowIndex === -1) {
                return this.getEmptyEducation();
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
        value: function saveItemAndCloseEditor(item, rowIndex) {
            var itemClone = $.extend({}, item, true);
            var items = $.extend([], this.state.items);
            if (rowIndex === -1) {
                items.push(itemClone);
            } else {
                items[rowIndex] = itemClone;
            }
            this.setState({ items: items });
            this.closeEditor();
        }
    }, {
        key: "render",
        value: function render() {
            var _this2 = this;

            var items = this.state.items.map(function (item, index) {
                return React.createElement(
                    "tr",
                    { key: item.Id, onClick: function onClick(e) {
                            return _this2.handleOnChange(index, e);
                        }, className: "education-item" },
                    React.createElement(
                        "td",
                        null,
                        index + 1
                    ),
                    _this2.props.readonly === true ? null : React.createElement(
                        "td",
                        null,
                        React.createElement(
                            "span",
                            { className: "label label-success", onClick: function onClick(e) {
                                    return _this2.showEditor(index);
                                } },
                            "\u0420\u0435\u0434\u0430\u043A\u0442\u0438\u0440\u043E\u0432\u0430\u0442\u044C"
                        ),
                        " ",
                        React.createElement(
                            "span",
                            { className: "label label-danger", onClick: function onClick(e) {
                                    return _this2.tryRemove(index);
                                } },
                            "\u0423\u0434\u0430\u043B\u0438\u0442\u044C"
                        )
                    ),
                    React.createElement(
                        "td",
                        null,
                        item.Inputs.map(function (itm) {
                            return React.createElement(
                                "div",
                                null,
                                itm.Text + ": " + itm.VisibleValue.toString()
                            );
                        })
                    )
                );
            });
            var educationItemEditor = null;
            if (this.state.editRowIndex !== null) {
                educationItemEditor = React.createElement(EducationItemEditor, {
                    key: this.state.editRequestId,
                    show: this.state.editRowIndex !== null,
                    onCancel: this.closeEditor,
                    onSave: function onSave(item) {
                        return _this2.saveItemAndCloseEditor(item, _this2.state.editRowIndex);
                    },
                    item: this.getItemByRowIndex(this.state.editRowIndex)
                });
            }
            return React.createElement(
                "div",
                null,
                this.props.readonly === true ? null : React.createElement(
                    "div",
                    null,
                    React.createElement(
                        "button",
                        { className: "btn btn-default", type: "button", onClick: function onClick(e) {
                                return _this2.showEditor(-1);
                            } },
                        "\u0414\u043E\u0431\u0430\u0432\u0438\u0442\u044C"
                    )
                ),
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
                                    "#"
                                ),
                                this.props.readonly === true ? null : React.createElement(
                                    "th",
                                    { width: "40px" },
                                    "\u041E\u043F\u0435\u0440\u0430\u0446\u0438\u0438"
                                ),
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
                            items
                        )
                    )
                ),
                educationItemEditor,
                React.createElement("input", { type: "hidden", value: JSON.stringify(this.state.items), name: this.props.name })
            );
        }
    }]);

    return EducationEditor;
}(React.Component);

$(document).ready(function () {
    $(".education-editor").each(function () {
        var $this = $(this);
        var editorName = $this.data("editorName");
        var isReadonly = $this.data("readonly");
        var items = $this.data("educations");
        var inputs = $this.data("inputs");
        var fileUploadUrl = $this.data("fileUploadUrl");

        ReactDOM.render(React.createElement(EducationEditor, { name: editorName, readonly: isReadonly, items: items, inputs: inputs }), this);
    });
});