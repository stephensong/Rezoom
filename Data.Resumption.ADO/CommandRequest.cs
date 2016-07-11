﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Resumption.DataRequests;
using Data.Resumption.Services;

namespace Data.Resumption.ADO
{
    public class CommandRequest : DataRequest<IReadOnlyList<CommandResponse>>
    {
        private readonly Command _command;

        public CommandRequest(Command command)
        {
            _command = command;
        }

        public override object Identity => FormattableString.Invariant(_command.Text);
        public override object DataSource => typeof(CommandBatch);
        public override bool Mutation => _command.Mutation;
        public override bool Idempotent => _command.Idempotent;
        public override object SequenceGroup => typeof(CommandBatch);

        public override Func<Task<IReadOnlyList<CommandResponse>>> Prepare(IServiceContext context)
            => context.GetService<CommandBatch>().Prepare(_command);
    }
}