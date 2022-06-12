# Ansible:
## 1. Схема работы:
- В общем случае управление происходит под пользователем devops.

- Управляющий сервер:
    * Это сервер с которого ведется управление остальными серверами через ansible; <br/>
    * Предварительные требования к системе: 
        - CentOs 7
        - наличие пользователя devops;
        - ssh сертификат для devops;
        - установлен ansible;
        <br/>
 
- Подконтрольные сервера:
    - CentOs 7

## 2. Настройка управляющего сервера
- Создание пользователя devops
    
        $ useradd -m devops 
        $ usermod -aG wheel devops
- Положить в /etc/sudoers.d/ запись  devops        ALL=(ALL)       NOPASSWD: ALL
        
- Генерация ssh сертификата
        
        $ su - devops
        $ ssh-keygen
        
- Установка ansible #взято отсюда https://docs.ansible.com/ansible/latest/installation_guide/intro_installation.html#latest-release-via-dnf-or-yum
        
        $ sudo yum install ansible
        
- Установка проекта ansible
    `расписать как установить проект ansible` `здесь надо понять как удобнее всего админам.`
    - либо скачать проект и распаковать
    - либо настроить автоматическое скачивание проекта с gitlab.
    <br/>

- Установка gitlab-runner (он должен работать из под devops)
    - yum install gitlab-runner '!!!Дописать!!! Мади посмотри'
    - изменяем настройки gitlab-runner.service (/etc/systemd/system/gitlab-runner.service)
        - изменить значение --working-directory -> /home/devops
        - изменить значение --user -> devops
    
## 3. Настройка подконтрольного сервера (ноды)
На управляющем сервере (далее все действия по умолчанию считать выполняемыми на управляющем сервере в текущей директории проекта ansible):
- Войдите под пользователем devops; 
- Перейти в директорию с проектом ansible;
- Добавить IP адрес (DNS имя) сервера в файл инвентаризации hosts; 
- Выполнить playbook  для добавления на подконтрольный сервер пользователя devops

        $ ansible-playbook init-devops-user-playbook.yml -i hosts -e "target={NODE_ADDRESS}" --become-user={BECOME_USER} --ask-pass -f 1

    где:
    * -i : дает возможность указать файл инвентаризации (в нашем случае это файл hosts)
    *  -e : дает возможность передать переменный в скрипты ansible;
         в нашем случае playbook требует переменную 
            target: для каких серверов выполнить playbook;

    *  --become-user: под каким пользователем установить ssh соединение с удаленным сервером
    *  --ask-pass: запрашивать пароль пользователя для установки ssh соединения
    *  -f: количество параллельных потоков
         в нашем случае лучше поставить 1, так как при соединении можеть потребовать подтвердить добавление fingerprint удаленного сервера в known_hosts, в мультипоточном режиме глючит.

    *  NODE_ADDRESS: IP (DNS) адрес подключаемого сервера
    * BECOME_USER: Пользователь с правами root (обычно это и есть root, если это не так то в предыдущий скрипт надо добавить параметр --become, чтобы команды выполнились с sudo);

    что выполнил playbook на удаленном сервере:
    * Добавил пользователя devops c домашней директорией
    * Добавил в known_hosts публичный сертификат пользателя devops текущей машины из которой выполняется playbook (/home/devops/.ssh/id_rsa.pub)
    * Добавил пользователя в sudoers и отключил вход по паролю 
    <br/>

	если на предыдущем этапе вышла ошибка, тогда самостоятельно войдите с управляющего сервера на подконтрольный с помощью ssh
    * Цель: добавить fingerprint подконтрольной машины в known_hosts;
    <br/>
- Выполнить playbook для пинга добавленного сервера
    
        $ ansible-playbook ping-playbook -i hosts -e "target={NODE_ADDRESS}"
    
  > *Возможно запросит подверждение на добавление fingerprint подконтрольного сервера в known_hosts. Подтвердите набрав yes.*

- Ура, товарищи! Ура!!! Подконтрольный сервер готов для возможности удаленного управления.

## 4. Инициализация web-сервера
На управляющем сервере:
- При необходимости выполните настройку подконтрольного сервера (см. 3);
- Войдите под пользователем devops; 

- Создать файл конфигурации веб-приложения (WEB_APP_CONFIG_FILE).
    - Создайте директорию для хранения конфигурации веб-серверов.
    - Скопируйте шаблон файла настроек vars-templates/webapp.yml в директорию настроек
    - Отредактируйте файл в соответствии с требованиями. 
    <br/>
- Перейти в директорию с проектом ansible
- Запустить init-web-server-playbook 

        $ ansible-playbook init-web-server-playbook.yml -i hosts -e "config_file={PATH_TO_WEB_APP_CONFIG_FILE} target={WEB_APP_SERVER_IP_OR_DNS_NAME}" -b
    
    
    где:
    * -i : дает возможность указать файл инвентаризации (в нашем случае это файл hosts)
    * -e : дает возможность передать переменный в скрипты ansible
      > в нашем случае playbook требует переменные <br/>
      > &nbsp;&nbsp; config_file: путь к файлу конфигурации веб-приложения WEB_APP_CONFIG_FILE <br/>
      > &nbsp;&nbsp; target: для каких серверов нужно выполнить playbook;
         <br/>
    * -b : сокращение от become, т.е. выполнять команды через sudo

    * PATH_TO_WEB_APP_CONFIG_FILE: полный путь к файлу конфигурации веб-приложения.
    * WEB_APP_SERVER_IP_OR_DNS_NAME: IP адрес или DNS имя сервера

	что делает playbook на удаленных серверах:
    * Устанавливает dotnet2
    * Устанавливает nginx
    * Открывает порт приложения для доступа снаружи
    * Создает два systemd сервиса
        - Создает рабочие каталоги для сервисов.
        - Создает .service файлы в /etc/systemd/system
        <br/>

- Почему такая структура? См. "Алгоритм обеспечения выкладки с zero downtime"

## 5. Выкладка обновлений на web-сервер
При необходимости выполните инициализацию веб-сервера (см. 4);
На управляющем сервере:
- Войдите под пользователем devops; 
- Перейти в директорию с проектом ansible;
- Запустить rollout-update-playbook.yml

        $ ansible-playbook -i hosts rollout-update-playbook.yml -e "config_file={PATH_TO_WEB_APP_CONFIG_FILE} target={WEB_APP_SERVER_IP_OR_DNS_NAME}" -b

    где:
    * -i : дает возможность указать файл инвентаризации (в нашем случае это файл hosts)
    * -e : дает возможность передать переменный в скрипты ansible;
        > в нашем случае playbook требует переменные <br/>
        > &nbsp;&nbsp; config_file: путь к файлу конфигурации веб-приложения WEB_APP_CONFIG_FILE <br/>
        > &nbsp;&nbsp; target: для каких серверов нужно выполнить playbook; 
    * -b : сокращение от become, т.е. выполнять команды через sudo

    * PATH_TO_WEB_APP_CONFIG_FILE: полный путь к файлу конфигурации веб-приложения.
    * WEB_APP_SERVER_IP_OR_DNS_NAME: IP адрес или DNS имя сервера

	что делает playbook на удаленных серверах:
    * Скачивает архив артифактов с gitlab;
    * Далее работает по алгоритм выкладки с zero downtime (см. Алгоритм обеспечения выкладки с zero downtime);

    * если rollout-update-playbook  не смог определить какой из сервисов нужно использовать для выкладки:
       передайте номер сервиса через параметр serviceNumber, например:

            $ ansible-playbook -i hosts rollout-update-playbook.yml -e "serviceNumber=1 config_file={PATH_TO_WEB_APP_CONFIG_FILE} target={WEB_APP_SERVER_IP_OR_DNS_NAME}" -b


# Алгоритм обеспечения выкладки с zero downtime:

Исходные данные:

- На сервере установлен nginx;
- 2 сервиса веб-приложения, которые слушают два разных порта;
- В запущенном состоянии находится только 1 самый свежий сервис (RUNNING_SERVICE);
- Второй сервис потушен (STOPPED_SERVICE);
- nginx проксирует на запущенный (RUNNING_SERVICE) сервис.

Алгоритм выкладки:


    1. Определить какой сервис запущен:                     DETECT RUNNING_SERVICE
    2. Определить какой сервис остановлен.                  DETECT STOPPED_SERVICE

    3. Выложить обновление на STOPPED_SERVICE               (STOPPED_SERVICE):
        Определить рабочий каталог                          (DETECT STOPPED_SERVICE_WORKING_DIR).
        Отчистить рабочий каталог                           (CLEAR STOPPED_SERVICE_WORKING_DIR)
        Распаковать архив обновления в рабочий каталог.     (Unarcheive UPDATES to STOPPED_SERVICE_WORKING_DIR)
        Запустить сервис                                    (START STOPPED_SERVICE)
        Ждать запуска порта                                 (WAIT_FOR STOPPED_SERVICE_PORT OPEN)
        Выполнить запрос на разогревочный url               (HTTP_GET STOPPED_SERVICE_WARMUP_URL)
        Переключить проксирование в nginx на
            STOPPED_SERVICE                                 (NGINX SWITCH TO STOPPED_SERVICE_PORT)
     
     4. Остановить RUNNING_SERVICE сервис                   (STOP RUNNING_SERVICE)