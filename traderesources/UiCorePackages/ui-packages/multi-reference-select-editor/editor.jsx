class MultiReferenceSelectEditor extends React.Component {
    constructor(props) {
        super(props);
        var selectedValuesDic = {};
        for (var i = 0; i < this.props.selectedValues.length; i++) {
            selectedValuesDic[this.props.selectedValues[i]] = true;
        }

        this.state = {
            selectedValuesDic: selectedValuesDic,
            checkStylesDic: this.computeCheckStylesDic(this.props.refItems, selectedValuesDic),
            readonly: this.props.readonly
        };
        this.computeCheckStylesDic = this.computeCheckStylesDic.bind(this);
        this.computeCheckStylesArr = this.computeCheckStylesArr.bind(this);
        this.toggleValue = this.toggleValue.bind(this);
        this.renderItems = this.renderItems.bind(this);
        this.flatten = this.flatten.bind(this);
        this.getGlyphIconClass = this.getGlyphIconClass.bind(this);
        this.flattenedRefItems = this.flatten(this.props.refItems);
    }

    flatten(refItems) {
        var ret = {};
        for (var i = 0; i < refItems.length; i++) {
            var curItem = refItems[i];
            ret[curItem.Code] = curItem;
            $.extend(ret, this.flatten(curItem.Items));
        }
        return ret;
    }

    computeCheckStylesDic(refItems, selectedValuesDic) {
        var checkStyles = this.computeCheckStylesArr(refItems, selectedValuesDic);
        var ret = {};
        for (var i = 0; i < checkStyles.length; i++) {
            ret[checkStyles[i].Code] = checkStyles[i].CheckStyle;
        }
        return ret;
    }

    computeCheckStylesArr(refItems, selectedValuesDic) {
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
                if (childrenCheckStyles.every(x => x.CheckStyle === "Checked")) {
                    curItemCheckStyle = "Checked";
                } else if (childrenCheckStyles.every(x => x.CheckStyle === "Unchecked")) {
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

    toggleValue(value) {
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
    getAllLeafItemsCodes(refItems) {
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
    getGlyphIconClass(checkStyle) {
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
    getCheckedCodes(selectedValuesDic) {
        var ret = [];
        for (var key in selectedValuesDic) {
            if (selectedValuesDic[key] === true) {
                ret.push(key);
            }
        }
        return ret;
    }

    renderItems(items) {
        if (items.length === 0) {
            return null;
        }
        return <ul className="ref-items">{items.map(item => <li><span className={this.getGlyphIconClass(this.state.checkStylesDic[item.Code])} onClick={e => this.toggleValue(item.Code)} aria-hidden="true"> </span><span onClick={e => this.toggleValue(item.Code)} className={("checkstyle-" + this.state.checkStylesDic[item.Code])}>{item.Text}</span> {this.renderItems(item.Items)}</li>)}</ul>;
    }

    render() {
        return (<div>
            {this.renderItems(this.props.refItems)}
            <input type="hidden" value={JSON.stringify(this.getCheckedCodes(this.state.selectedValuesDic))} name={this.props.name} />
        </div>);
    }
}

$(document).ready(function () {
    $(".multi-reference-select-editor").each(function () {
        const $this = $(this);
        const editorName = $this.data("editorName");
        const isReadonly = $this.data("readonly");
        const items = $this.data("items");
        const selectedValues = $this.data("selectedValues");
        ReactDOM.render(
            <MultiReferenceSelectEditor name={editorName} readonly={isReadonly} refItems={items} selectedValues={selectedValues} />,
            this
        );
    });
});