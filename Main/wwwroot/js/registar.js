document.addEventListener("DOMContentLoaded", function () {
    const form = document.querySelector(".kt-login-v2__form");
    const telefoneInput = form.telefone;

    // Permitir apenas a entrada de números
    telefoneInput.addEventListener("input", function (event) {
        this.value = this.value.replace(/\D/g, '');
    });

    form.addEventListener("submit", function (event) {
        event.preventDefault(); // Impede o envio padrão do formulário

        // Obtém os valores dos campos
        const fullname = form.fullname.value.trim();
        const email = form.email.value.trim();
        const username = form.username.value.trim();
        const telefone = form.telefone.value.trim();
        const password = form.password.value.trim();
        const confirmPassword = form.confirm_password.value.trim();

        // Verifica se todos os campos estão preenchidos
        if (!fullname || !email || !username || !telefone || !password || !confirmPassword) {
            swal.fire({
                type: 'error',
                title: 'Oops...',
                text: 'Por favor, preencha todos os campos!',
                timer: 3000,
                showConfirmButton: false,
                onOpen: () => {
                    swal.showLoading();
                }
            });
            return;
        }

        // Verifica se o e-mail é válido
        const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailPattern.test(email)) {
            swal.fire({
                type: 'error',
                title: 'E-mail inválido',
                text: 'Por favor, insira um e-mail válido.',
                timer: 3000,
                showConfirmButton: false,
                onOpen: () => {
                    swal.showLoading();
                }
            });
            return;
        }

        // Verifica se o telefone tem 9 dígitos
        if (telefone.length !== 9) {
            swal.fire({
                type: 'error',
                title: 'Telefone inválido',
                text: 'O número de telefone deve ter 9 dígitos.',
                timer: 3000,
                showConfirmButton: false,
                onOpen: () => {
                    swal.showLoading();
                }
            });
            return;
        }

        // Verifica se a senha e a confirmação são iguais
        if (password !== confirmPassword) {
            swal.fire({
                type: 'error',
                title: 'Senhas não coincidem',
                text: 'A senha e a confirmação devem ser iguais.',
                timer: 3000,
                showConfirmButton: false,
                onOpen: () => {
                    swal.showLoading();
                }
            });
            return;
        }

        // Validação da senha
        if (!validatePassword(password)) {
            swal.fire({
                type: 'error',
                title: 'Senha inválida',
                text: 'A senha deve ter pelo menos 6 caracteres, incluir pelo menos 2 números e 1 caractere especial.',
                timer: 3000,
                showConfirmButton: false,
                onOpen: () => {
                    swal.showLoading();
                }
            });
            return;
        }

        // Se todas as validações passarem, continuar com o registro
        const formData = {
            fullname,
            email,
            username,
            telefone,
            password
        };

        // Enviar dados para o controlador usando fetch
        fetch('/Home/CriarConta', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(formData)
        })
            .then(response => {
                if (response.ok) {
                    return response.json(); // Converte a resposta para JSON
                }
                throw new Error('Erro na requisição');
            })
            .then(data => {
                if (data.success) {
                    swal.fire({
                        type: 'success',
                        title: 'Registro bem-sucedido',
                        text: 'Sua conta foi criada com sucesso!',
                        timer: 3000,
                        showConfirmButton: false,
                        onOpen: () => {
                            swal.showLoading();
                        }
                    }).then(() => {
                        form.reset(); // Limpa o formulário
                        window.location.href = '/'; // Redireciona para a página de login
                    });

                } else {
                    swal.fire({
                        type: 'error',
                        title: 'Erro',
                        text: data.message,
                        timer: 3000,
                        showConfirmButton: false,
                        onOpen: () => {
                            swal.showLoading();
                        }
                    });
                }
            })
            .catch(error => {
                console.error('Erro:', error);
                swal.fire({
                    type: 'error',
                    title: 'Erro na comunicação',
                    text: 'Ocorreu um erro ao tentar se registrar.',
                    timer: 3000,
                    showConfirmButton: false,
                    onOpen: () => {
                        swal.showLoading();
                    }
                });
            });
    });

    // Função de validação da senha
    function validatePassword(password) {
        const minLength = 6;
        const numberPattern = /[0-9]/g; // Expressão regular para números
        const specialCharPattern = /[!@#$%^&*(),.?":{}|<>]/g; // Expressão regular para caracteres especiais

        // Verifica se a senha tem pelo menos 6 caracteres
        if (password.length < minLength) return false;

        // Verifica se a senha tem pelo menos 2 números
        const numbers = password.match(numberPattern);
        if (!numbers || numbers.length < 2) return false;

        // Verifica se a senha contém pelo menos 1 caractere especial
        if (!specialCharPattern.test(password)) return false;

        return true; // A senha atende todos os critérios
    }
});
