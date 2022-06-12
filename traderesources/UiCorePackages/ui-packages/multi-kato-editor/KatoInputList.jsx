class KatoInputList extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            asyncRequestRunning: false,
            asyncRequestId: 0,
            asyncRequestError: null,
            inputs: this.props.inputs,
            country: this.props.kato.filter(x => x.ParentId === null),
            obl: this.props.kato,
            region: this.props.kato,
        };
        this.onChange = this.onChange.bind(this);
        this.onSave = this.onSave.bind(this);
        this.validate = this.validate.bind(this);
        this.isNull = this.isNull.bind(this);
        this.isNullOrEmpty = this.isNullOrEmpty.bind(this);
        this.validateRanges = this.validateRanges.bind(this);
    }
    onChange(name, val) {
        var inputs = $.extend(true, [], this.state.inputs);
        var input = inputs.find(x => x.Name === name);
        input.Value = val;
        input.Error = this.validate(input);
        var generalError = this.state.generalError;
        if (inputs.every(input => this.isNullOrEmpty(input.Error))) {
            generalError = null;
        }
        var obls = this.state.obl;
        var regions = this.state.region;
        if (name === "AdrCountry" && val !== null) {
            obls = this.props.kato.filter(x => x.ParentId === val).sort();
        }
        if (name === "AdrObl" && val !== null) {
            regions = this.props.kato.filter(x => x.ParentId === val).sort();
        }
        this.setState({ inputs: inputs, generalError: generalError, obl: obls, region: regions });
    }
    onSave() {
        var inputs = $.extend(true, [], this.state.inputs);

        inputs.forEach(input => { input.Error = this.validate(input); });
        this.validateRanges(
            input => input.Name,
            inputs.filter(input => this.isNullOrEmpty(input.Error))
        );
        this.validateRanges(
            input => input.Alias,
            inputs.filter(input => this.isNullOrEmpty(input.Error) && !this.isNullOrEmpty(input.Alias))
        );

        if (inputs.some(input => !this.isNullOrEmpty(input.Error))) {
			this.setState({
				inputs: inputs, generalError: inputs.map(input => {
					if (!this.isNullOrEmpty(input.Error)) {
						return input.Text + ": " + input.Error + "; "
					}
				}) });
        } else {
            this.props.onSave(inputs);
        }
    }
    validateRanges(getInputNameLambda, inputs) {
        var fromSuffix = "From";
        var toSuffix = "To";
        
        var froms = inputs.filter(input => getInputNameLambda(input).endsWith(fromSuffix));
        froms.forEach(from => {
            var fromInputName = getInputNameLambda(from);
            var inputToName = fromInputName.substring(0, fromInputName.length - fromSuffix.length) + toSuffix;
            var inputTo = inputs.find(input => getInputNameLambda(input) === inputToName);

            if (inputTo) {
                if (inputTo.Value < from.Value) {
                    inputTo.Error = "Значение должно быть больше, чем \"" + from.Text + "\"";
                }
            }
        });
    }
    isNull(val) {
        if (val === undefined || val === null) {
            return true;
        }
        return false;
    }
    isNullOrEmpty(val) {
        if (this.isNull(val)) {
            return true;
        }
        return val === "";
    }
    validate(input) {
        if (input.InputType === "File") {
            return null;
        }
        if (!input.IsRequired) {
            return null;
        }

        if (!input.Value && !input.IsHidden) {
            return "поле обязательно для заполнения";
        }

        return null;
    }
    render() {
        if (!this.props.show) {
            return null;
        }

        const Modal = ReactBootstrap.Modal;
        const Button = ReactBootstrap.Button;
        var inputs = this.state.inputs;
        var country = inputs.find(x => x.Name === "AdrCountry");
        var obl = inputs.find(x => x.Name === "AdrObl");
        var region = inputs.find(x => x.Name === "AdrReg");
        var adr = inputs.find(x => x.Name === "AdrAdr");
        var adrKz = inputs.find(x => x.Name === "AdrAdrKz");
        var phone = inputs.find(x => x.Name === "Phone");
        var email = inputs.find(x => x.Name === "Email");
        var kadNumber = inputs.find(x => x.Name === "CadNumber");
        var generalErrorPanel = null;
        if (this.state.generalError) {
			generalErrorPanel = <div className="alert alert-danger" role="alert">{this.state.generalError}</div>;
        }
        var emptyOption = <option>Выберите значение</option>;
        var cadNumberInput = null;
        var adrKzInput = null;
        var phoneInput = null;
        var emailInput = null;
        if (phone !== undefined) {
            phoneInput = <div className="form-group" >
                <label>Телефон</label>
                <input className="form-control" onChange={e => this.onChange(phone.Name, e.target.value)} value={phone.Value} disabled={this.props.blocked} name="phone" key="phone" />
            </div>;
		}
        if (email !== undefined) {
            emailInput = <div className="form-group" >
                <label>E-mail</label>
                <input className="form-control" onChange={e => this.onChange(email.Name, e.target.value)} value={email.Value} disabled={this.props.blocked} name="email" key="email" />
            </div>;
		}
        if (adrKz !== undefined) {
            adrKzInput = <div className="form-group" >
                <label>Адрес (на казахском)</label>
                <input className="form-control" onChange={e => this.onChange(adrKz.Name, e.target.value)} value={adrKz.Value} disabled={this.props.blocked} name="AdrAdrKz" key="AdrAdrKz" />
            </div>;
		}
        if (kadNumber !== undefined) {
            cadNumberInput = <div className="form-group">
                <label>Кадастровый номер объекта недвижимости, на котором расположен субъект аккредитации</label>
                <SearchCollectionEditor twoFielded={true} name={kadNumber.Name} dontPutHiddenInput onChange={e => this.onChange(kadNumber.Name, e)} value={kadNumber.Value} searchCollectionUrl={kadNumber.ModelCollectionUrl} searchDialogHeaderText="Выбор" queryInputPlaceholder="БИН организации;Кадастровый номер" key={kadNumber.Name} />
            </div>;
		}
        return (
            <div>
                <Modal show backdrop="static" onHide={this.props.onCancel}>
                    <Modal.Header closeButton>
                        <Modal.Title id="contained-modal-title-lg">{this.props.text}</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        {generalErrorPanel}
                        <div>
                            <div className="form-group">
                                <label>Страна</label>
                                <select className="form-control" onChange={e => this.onChange("AdrCountry", e.target.value)} value={country.Value} disabled={this.props.blocked} name="AdrCountry" key="AdrCountry">
                                    {emptyOption}
                                    {this.state.country !== null ? this.state.country.map(i => <option value={i.Id}>{i.Text}</option>) : ""}
                                </select>
                            </div>
                            <div className="form-group">
                                <label>Область</label>
                                <select className="form-control" onChange={e => this.onChange("AdrObl", e.target.value)} value={obl.Value} disabled={this.props.blocked} name="AdrObl" key="AdrObl">
                                    {emptyOption}
                                    {this.state.obl !== null ? this.state.obl.map(i => <option value={i.Id}>{i.Text}</option>) : ""}
                                </select>
                            </div>
                            <div className="form-group">
                                <label>Регион</label>
                                <select className="form-control" onChange={e => this.onChange("AdrReg", e.target.value)} value={region.Value} disabled={this.props.blocked} name="AdrReg" key="AdrReg">
                                    {emptyOption}
                                    {this.state.region !== null ? this.state.region.map(i => <option value={i.Id}>{i.Text}</option>) : ""}
                                </select>
                            </div>
                            <div className="form-group" >
                                <label>Адрес</label>
                                <input className="form-control" onChange={e => this.onChange(adr.Name,e.target.value)} value={adr.Value} disabled={this.props.blocked} name="AdrAdr" key="AdrAdr" />
                            </div>
                            {phoneInput}
                            {emailInput}
                            {adrKzInput}
                            {cadNumberInput}
                        </div>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button onClick={this.onSave}>Добавить</Button>
                        <Button onClick={this.props.onCancel}>Отмена</Button>
                    </Modal.Footer>
                </Modal>
            </div>
        );
    }
}