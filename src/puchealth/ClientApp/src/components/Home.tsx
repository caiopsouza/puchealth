import * as React from 'react';


export default class Home extends React.PureComponent<{ history: any }, {}> {
    constructor(props: any) {
        super(props);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    public state = {
        email: '',
        password: '',
        error: false
    };

    public async handleSubmit(event: any) {
        event.preventDefault();

        const response = await fetch('https://localhost:5001/v1/Account/login', {
            method: 'POST',
            headers: {'Accept': 'application/json', 'Content-Type': 'application/json'},
            body: JSON.stringify({email: this.state.email, password: this.state.password})
        });

        if (response.status == 401) {
            this.setState({error: true});
            return;
        }

        this.setState({error: false});
        this.props.history.push('/dashboard')
    }

    public render() {
        return (
            <div>
                <div className="bg-blue-400 h-screen w-screen">
                    <div className="flex flex-col items-center flex-1 h-full justify-center px-4 sm:px-0">
                        <div className="flex rounded-lg shadow-lg w-full sm:w-3/4 lg:w-1/2 bg-white sm:mx-0"
                             style={{height: '500px'}}>
                            <div className="flex flex-col w-full md:w-1/2 p-4">
                                <div className="flex flex-col flex-1 justify-center mb-8">
                                    <h1 className="text-4xl text-center font-thin">Seja bem-vindo!</h1>
                                    <div className="w-full mt-4">
                                        <form className="form-horizontal w-3/4 mx-auto"
                                              onSubmit={this.handleSubmit}>
                                            <div className="flex flex-col mt-4">
                                                <input id="email" type="text"
                                                       className="flex-grow h-8 px-2 border rounded border-grey-400"
                                                       name="email" value={this.state.email} placeholder="Email"
                                                       onChange={(event) => this.setState({email: event.target.value})}
                                                />
                                            </div>
                                            <div className="flex flex-col mt-4">
                                                <input id="password" type="password"
                                                       className="flex-grow h-8 px-2 rounded border border-grey-400"
                                                       name="password" required placeholder="Senha"
                                                       value={this.state.password}
                                                       onChange={(event) => this.setState({password: event.target.value})}/>
                                            </div>
                                            <p style={{
                                                color: 'red',
                                                paddingTop: '10px'
                                            }}>{this.state.error && 'Cadastro não encontrado no sistema. Favor verificar.'}</p>
                                            <div className="flex items-center mt-4">
                                                <input type="checkbox" value="" name="remember"
                                                       id="remember" className="mr-2"/>
                                                <label
                                                    htmlFor="remember" className="text-sm text-grey-dark">Lembrar de
                                                    mim</label>
                                            </div>
                                            <div className="flex flex-col mt-8">
                                                <button type="submit"
                                                        className="bg-blue-500 hover:bg-blue-700 text-white text-sm font-semibold py-2 px-4 rounded">
                                                    Login
                                                </button>
                                            </div>
                                        </form>
                                        <div className="text-center mt-4">
                                            <a className="no-underline hover:underline text-blue-dark text-xs"
                                               href="{{ route('password.request') }}">
                                                Esqueceu sua senha?
                                            </a>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div className="hidden md:block md:w-1/2 rounded-r-lg"
                                 style={{
                                     background: "url('https://images.unsplash.com/photo-1515965885361-f1e0095517ea?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=3300&q=80')",
                                     backgroundSize: 'cover',
                                     backgroundPosition: 'center center',
                                 }}></div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}