class DocsItemEditor extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            asyncRequestRunning: false,
            asyncRequestId: 0,
            asyncRequestError: null,
            item: this.props.item
        };
        this.onChange = this.onChange.bind(this);
        this.runModelSync = this.runModelSync.bind(this);
    }
    onChange(name, val) {
        var item = $.extend({}, this.state.item, true);
        var input = item.Inputs.find(x => x.Name === name);
        input.Value = val;
        this.setState({ item: item });
    }

    onChange(name, val) {
        var item = $.extend({}, this.state.item, true);
        var input = item.Inputs.find(x => x.Name === name);
        input.Value = val;
        this.setState({ item: item });
    }

    render() {
        if (!this.props.show) {
            return null;
        }

        const Modal = ReactBootstrap.Modal;
        const Button = ReactBootstrap.Button;

        return (
            <div>
                <Modal show={true} onHide={this.props.onCancel}>
                    <Modal.Header>
                        <Modal.Title id="contained-modal-title-lg">Данные о документах</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>

                        <div>
                            {this.state.item.Inputs.map(i => {
                                return (<div className={"form-group"} key={i.Id}>
                                    <label htmlFor="loan-item-name">{i.Text}</label>
                                    <DocsInputItem item={i} onChange={val => this.onChange(i.Name, val)} />
                                    {i.Errors.map(error => <span className="input-error-msg text-danger">{error}</span>)}
                                </div>);
                            }
                            )}
                        </div>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button onClick={e => this.props.onSave(this.state.item)}>ОК</Button>
                        <Button onClick={this.props.onCancel}>Отмена</Button>
                    </Modal.Footer>
                </Modal>
            </div>
        );
    }
}