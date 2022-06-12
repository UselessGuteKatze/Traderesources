class KatoItemList extends React.Component {
    constructor(props) {
        super(props);
        var items = this.props.items;
        if (items === undefined || items === null) {
            items = [];
        }
        items = items.map(x => {
            var inputs = $.extend(true, [], this.props.inputs);
            for (var i = 0; i < inputs.length; i++) {
                var input = inputs[i];
                input.Value = x[input.Name];
                this.prepareInput(input);
            }
            return { Inputs: inputs };
        });
        this.state = {
            items: items,
            inputs: this.props.inputs,
            showSelectModal: false,
            editRowIndex: null,
            editRequestId: 0
        };
        this.prepareInput = this.prepareInput.bind(this);
        this.getDateFromStr = this.getDateFromStr.bind(this);
        this.getEmptyItem = this.getEmptyItem.bind(this);
        this.getItemByRowIndex = this.getItemByRowIndex.bind(this);
        this.showEditor = this.showEditor.bind(this);
        this.closeEditor = this.closeEditor.bind(this);
        this.saveItemAndCloseEditor = this.saveItemAndCloseEditor.bind(this);
    }
    prepareInput(input) {
        input.VisibleValue = input.Value;
        if (input.Name === "AdrCountry" || input.Name === "AdrObl" || input.Name === "AdrReg") {
            if (input.VisibleValue !== null) {
                input.VisibleValue = this.props.kato.find(x => x.Id === input.Value).Text;
			}
            
        }
        if (input.InputType === "CollectionModel" && input.Value) {
            input.VisibleValue = input.Value.SearchItemText;
        }
        if (input.VisibleValue === null || input.VisibleValue === undefined) {
            input.VisibleValue = "";
        }
    }
    getDateFromStr(newtonsoftJsonSerializedDateString) {
        if (newtonsoftJsonSerializedDateString instanceof Date) {
            return newtonsoftJsonSerializedDateString;
        }
        return new Date(Date.parse(newtonsoftJsonSerializedDateString));
    }
    
    getEmptyItem() {
        var inputs = $.extend(true, [], this.props.inputs);
        for (var i = 0; i < inputs.length; i++) {
            this.prepareInput(inputs[i]);
        }
        return {
            Inputs: inputs
        };
    }

    getItemByRowIndex(rowIndex) {
        if (rowIndex === -1) {
            return this.getEmptyItem();
        } else {
            return this.state.items[rowIndex];
        }
    }
    showEditor(rowIndex) {
        this.setState({ editRowIndex: rowIndex, editRequestId: this.state.editRequestId + 1 });
    }
    tryRemove(rowIndex) {
        if (confirm("Удалить запись?") !== true) {
            return;
        }
        var items = $.extend(true, [], this.state.items);
        items.splice(rowIndex, 1);
        this.setState({ items: items });
    }
    closeEditor() {
        this.setState({ editRowIndex: null });
    }
    saveItemAndCloseEditor(inputs, rowIndex) {
        inputs = $.extend(true, [], inputs);
        inputs.forEach(input => this.prepareInput(input));
        var items = $.extend(true, [], this.state.items);
        if (rowIndex === -1) {
            items.push({Inputs: inputs});
        } else {
            items[rowIndex].Inputs = inputs;
        }
        this.setState({ items: items });
        this.closeEditor();
    }
    getOpsCellIfNotReadonly(rowIndex) {
        if (this.props.readonly) {
            return null;
        }
        return (<td>
            <span className="badge badge-success cursor-pointer" onClick={e => this.showEditor(rowIndex)}>Редактировать</span>
            <span>&nbsp;</span>
            <span className="badge badge-danger cursor-pointer" onClick={e => this.tryRemove(rowIndex)}>Удалить</span>
        </td>);
    }
    render() {
        const items = this.state.items.map((item, index) => 
            item.Name !== "FullAdr" ?
            (<tr key={item.Id} className="custom-item">
                <td>{index + 1}</td>
                {this.getOpsCellIfNotReadonly(index)}
                <td>{item.Inputs.map((itm) => itm.Name !== "FullAdr" ? (<div><b>{itm.Text}</b>{": " + itm.VisibleValue.toString()}</div>) : "")}</td>
            </tr>) : ""
        );
        var empty = null;
        if (this.state.items.length === 0) {
            empty = <tr key={"empty"} className="empty-item"><td colSpan={(this.props.readonly === true ? 2: 3)}>Нет записей</td></tr>;
        }

        var customItemEditor = null;
        if (this.state.editRowIndex !== null) {
            customItemEditor = <KatoInputList show key={this.state.editRequestId} text={this.props.text} kato={this.props.kato} inputs={this.getItemByRowIndex(this.state.editRowIndex).Inputs} onCancel={this.closeEditor} onSave={inputs => this.saveItemAndCloseEditor(inputs, this.state.editRowIndex)} />;
        }
        var val = this.state.items.map(x => {
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
            addBtn = (<div>
                <button className="btn btn-primary" type="button" onClick={e => this.showEditor(-1)}>Добавить</button>
            </div>);
            operationsHeaderCell = <th width="180px">Операции</th>;
        }
        return (
            <div>
                {addBtn}
                <div>
                    <table className="table">
                        <thead>
                            <tr>
                                <th width="20px">№</th>
                                {operationsHeaderCell}
                                <th>Данные</th>
                            </tr>
                        </thead>
                        <tbody>
                            {items}
                            {empty}
                        </tbody>
                    </table>
                </div>
                {customItemEditor}
                <input type="hidden" value={JSON.stringify(val)} name={this.props.name} />
            </div>
        );
    }
}

$(document).ready(function () {
    $(".kato-object-list-editor").each(function () {
        const $this = $(this);
        const editorName = $this.data("editorName");
        const isReadonly = $this.data("readonly");
        const items = $this.data("items");
        const inputs = $this.data("inputs");
        const kato = $this.data("kato");
        const text = $this.data("text");
        ReactDOM.render(
            <KatoItemList name={editorName} text={text} readonly={isReadonly} items={items} inputs={inputs} kato={kato}/>,
            this
        );
    });
});