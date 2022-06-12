class EducationItemEditor extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            asyncRequestRunning: false,
            asyncRequestId: 0,
            asyncRequestError: null,
            item: this.props.item
        };
        this.onChange = this.onChange.bind(this);
    }
    onChange(name, val) {
        var item = $.extend({}, this.state.item, true);
        var input = item.Inputs.find(x => x.Name === name);
        input.Value = val;
        input.VisibleValue = val;
        if (input.InputType === "Reference" || input.InputType === "Reference") {
            input.VisibleValue = input.RefItems.find(x => x.Code === val).Text;
        }
        if (input.InputType === "File") {
            input.VisibleValue = input.Value.map(i => i.FileName).join(",");
        }
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
                <Modal show={this.props.show} onHide={this.props.onCancel}>
                    <Modal.Header>
                        <Modal.Title id="contained-modal-title-lg">Данные об образовании</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>

                        <div>
                            {this.state.item.Inputs.map(i => {
                                return (<div className={"form-group"} key={i.Id}>
                                    <label htmlFor="loan-item-name">{i.Text}</label>
                                    <EducationInputItem item={i} onChange={val => this.onChange(i.Name, val)} />
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