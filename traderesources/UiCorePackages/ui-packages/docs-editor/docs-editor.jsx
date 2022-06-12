class DocsEditor extends React.Component {
    constructor(props) {
        super(props);
        var items = this.props.items;
        if (items === undefined || items === null) {
            items = [];
        }

        this.state = {
            items: items,
            showSelectModal: false,
            editRowIndex: null,
            editRequestId: 0
        }
        this.getEmptyDoc = this.getEmptyDoc.bind(this);
        this.getItemByRowIndex = this.getItemByRowIndex.bind(this);
        this.showEditor = this.showEditor.bind(this);
        this.closeEditor = this.closeEditor.bind(this);
        this.saveItemAndCloseEditor = this.saveItemAndCloseEditor.bind(this);
    }

    getEmptyDoc() {
        return {
            Docs: {

            },
            Inputs: []
        };
    }

    getItemByRowIndex(rowIndex) {
        if (rowIndex === -1) {
            return this.getEmptyDoc();
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
    saveItemAndCloseEditor(item, rowIndex) {
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

    render() {
        const items = this.state.items.map((item, index) => (
            <tr key={item.Id} onClick={e => this.handleOnChange(index, e)} className="education-item">
                <td>{index + 1}</td>
                {
                    this.props.readonly === true
                        ? null
                        : <td><span className="label label-success" onClick={e => this.showEditor(index)}>Редактировать</span> <span className="label label-danger" onClick={e => this.tryRemove(index)}>Удалить</span></td>
                }
            </tr>
        ));
        var docsItemEditor = null;
        if (this.state.editRowIndex !== null) {
            docsItemEditor = (<DocsItemEditor
                key={this.state.editRequestId}
                show={(this.state.editRowIndex !== null)}
                onCancel={this.closeEditor}
                onSave={item => this.saveItemAndCloseEditor(item, this.state.editRowIndex)}
                item={this.getItemByRowIndex(this.state.editRowIndex)}
            />);
        }
        return (
            <div>
                {
                    this.props.readonly === true
                        ? null
                        : <div>
                            <button className="btn btn-primary" type="button" onClick={e => this.showEditor(-1)}>Добавить</button>
                        </div>
                }
                <div>
                    <table className="table">
                        <thead>
                            <tr>
                                <th width="20px">#</th>
                                {
                                    this.props.readonly === true
                                        ? null
                                        : <th width="40px">Операции</th>
                                }

                                <th>Данные</th>
                            </tr>
                        </thead>
                        <tbody>
                            {items}
                        </tbody>
                    </table>
                </div>
                {docsItemEditor}
                <input type="hidden" value={JSON.stringify(this.state.items)} name={this.props.name} />
            </div>
        );
    }
}

$(document).ready(function () {
    $(".docs-editor").each(function () {
        const $this = $(this);
        const editorName = $this.data("editorName");
        const isReadonly = $this.data("readonly");
        const items = $this.data("docs");

        ReactDOM.render(
            <DocsEditor name={editorName} readonly={isReadonly} items={items}/>,
            this
        );
    });
});